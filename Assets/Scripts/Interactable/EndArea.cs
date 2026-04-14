using UnityEngine;
using UnityEngine.UI;

public class EndArea : MonoBehaviour
{
    [SerializeField] GameObject player1Entered, player2Entered;
    [SerializeField] private float percentageToEnd;
    [SerializeField] private float timeToFill = 3f;

    [SerializeField] private GameObject canvasEndArea;
    [SerializeField] private Slider sliderEndArea;
    [SerializeField] private Image sliderFill;
    [SerializeField] Color colorSlider, colorEndScreen;

    void Start()
    {
        player1Entered = null;
        player2Entered = null;
        sliderFill.color = colorSlider;
    }

    void Update()
    {
        UpdatePercentIfCan();
        UpdateCanvasVisibility();
        UpdateSlider();
        UpdateSliderSize();
    }

    void UpdatePercentIfCan()
    {
        if (!GameManager.CanPlayersReturnToEndArea()) return;
        if (GameManager.Instance.GetCurrentGameStateTag() != GameManager.GameStateTag.StartGame) return;

        if (AreBothPlayersInEndArea())
        {
            percentageToEnd += Time.deltaTime / timeToFill;
            if (percentageToEnd >= 1f)
            {
                GameManager.Instance.WonGame();
            }
        }
        else
        {
            percentageToEnd -= Time.deltaTime / timeToFill;
        }
        percentageToEnd = Mathf.Clamp01(percentageToEnd);
    }

    void UpdateCanvasVisibility()
    {
        if (percentageToEnd >= 0.01f)
        {
            canvasEndArea.SetActive(true);
            return;
        }

        if (GameManager.CanPlayersReturnToEndArea() && AreBothPlayersInEndArea())
        {
            canvasEndArea.SetActive(true);
        }
        else
        {
            canvasEndArea.SetActive(false);
        }
    }

    void UpdateSlider()
    {
        if (sliderEndArea.value < 1)
            sliderEndArea.value = Mathf.MoveTowards(sliderEndArea.value, percentageToEnd, Time.deltaTime * 5f);
    }

    void UpdateSliderSize()
    {
        if (sliderEndArea.value >= 1)
        {
            sliderEndArea.transform.localScale = Vector3.Lerp(sliderEndArea.transform.localScale, Vector2.one * 150, Time.deltaTime * 0.25f);
            sliderFill.color = Color.Lerp(sliderFill.color, colorEndScreen, Time.deltaTime * 3f);
        }
    }

    bool AreBothPlayersInEndArea() => player1Entered != null && player2Entered != null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (player.GetPlayerTag() == PlayerTag.Player1)
            {
                player1Entered = player.gameObject;
            }
            else if (player.GetPlayerTag() == PlayerTag.Player2)
            {
                player2Entered = player.gameObject;
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (player.GetPlayerTag() == PlayerTag.Player1)
            {
                player1Entered = null;
            }
            else if (player.GetPlayerTag() == PlayerTag.Player2)
            {
                player2Entered = null;
            }
        }
    }
}
