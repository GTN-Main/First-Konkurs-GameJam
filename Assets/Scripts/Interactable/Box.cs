using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(DetectPlayerInRange), typeof(BoxProgressOpen), typeof(BoxUI))]
public class Box : MonoBehaviour
{
    DetectPlayerInRange detectPlayerInRange;
    BoxProgressOpen boxProgressOpen;
    BoxUI boxUI;

    [SerializeField]
    float progressSpeed = 0.05f;

    [SerializeField]
    float progressDecreaseSpeed = 0.1f;

    [SerializeField]
    float maxProgress = 1f;

    [SerializeField]
    Animator boxAnimator;

    [SerializeField]
    AnimationClip landAnimationClip;

    private GameObject spawnPoint;

    public void SetSpawnPoint(GameObject spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public GameObject GetSpawnPoint()
    {
        return spawnPoint;
    }

    void Start()
    {
        detectPlayerInRange = GetComponent<DetectPlayerInRange>();
        boxProgressOpen = GetComponent<BoxProgressOpen>();
        boxUI = GetComponent<BoxUI>();
        detectPlayerInRange.SetDetectionUsability(true);
        MyAudioEffects.Instance.DoEffect("BoxSpawn", transform.position, 1f);
    }

    void Update()
    {
        if (!isNowOpening && !isBoxOpened)
            WhenBoxIsNotOpeningAndIsNotOpened();
        else if (isNowOpening && !isBoxOpened)
            WhenBoxIsOpeningButNotOpened();
    }

    void WhenBoxIsNotOpeningAndIsNotOpened()
    {
        int playersInRangeCount = detectPlayerInRange.GetPlayersInRangeCount();
        if (playersInRangeCount == 0)
        {
            boxUI.SetText("");
            boxUI.GetTargetCanvas().SetActive(false);
            return;
        }

        if (playersInRangeCount == detectPlayerInRange.GetPlayersNeedCount())
        {
            boxUI.SetText(
                $"{playersInRangeCount}/{detectPlayerInRange.GetPlayersNeedCount()}\nInteract"
            );
        }
        else
        {
            boxUI.SetText($"{playersInRangeCount}/{detectPlayerInRange.GetPlayersNeedCount()}");
        }
        boxUI.GetTargetCanvas().SetActive(playersInRangeCount > 0);
        if (detectPlayerInRange.IsActiveted())
            Open();
    }

    void WhenBoxIsOpeningButNotOpened()
    {
        if (detectPlayerInRange.IsActiveted())
            boxProgressOpen.StartIncreasingProgress();
        else
            boxProgressOpen.StartDecreasingProgress();

        if (
            detectPlayerInRange.GetPlayersInRangeCount()
            != detectPlayerInRange.GetPlayersNeedCount()
        )
            boxProgressOpen.InteruptOpening();
    }

    [SerializeField]
    bool isNowOpening = false;

    [SerializeField]
    bool isBoxOpened = false;

    async Task Open()
    {
        if (isNowOpening || isBoxOpened)
            return;
        isNowOpening = true;
        void OnProgressChanged(float newProgress)
        {
            boxUI.SetText($"{newProgress:P0}");
        }

        boxProgressOpen.onProgressChanged += OnProgressChanged;
        boxProgressOpen.onBoxOpened += BoxOpened;
        boxProgressOpen.onOpeningTryEnded += OnOpeningTryEnded;

        await boxProgressOpen.OpenBox(true, progressSpeed, progressDecreaseSpeed, maxProgress);

        boxProgressOpen.onProgressChanged -= OnProgressChanged;
        boxProgressOpen.onBoxOpened -= BoxOpened;
        boxProgressOpen.onOpeningTryEnded -= OnOpeningTryEnded;

        isNowOpening = false;
    }

    void BoxOpened()
    {
        Debug.Log("Box opened! Implement reward logic here.");
        isBoxOpened = true;
        boxUI.GetTargetCanvas().SetActive(true);
        detectPlayerInRange.SetDetectionUsability(false);
        boxUI.SetText("Box Opened!");
        onBoxOpened?.Invoke();
        MyAudioEffects.Instance.DoEffect("BoxOpenSound", transform.position, 1f);
    }

    public event System.Action onBoxOpened;

    void OnOpeningTryEnded()
    {
        // Debug.Log("Opening attempt ended. Implement any necessary logic here.");
    }

    public void BeginLandAnimation()
    {
        if (boxAnimator != null && landAnimationClip != null)
        {
            boxAnimator.Play(landAnimationClip.name);
        }
    }
}
