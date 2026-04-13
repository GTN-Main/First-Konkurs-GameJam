using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxProgressOpen : MonoBehaviour
{
    InputAction openActionTest;
    InputAction resetActionTest;
    [SerializeField] bool isIncreasing = true;
    OpenProgress openProgress;
    [SerializeField, Range(0, 1)] float progress;

    [SerializeField] bool isOpening;

    void Start()
    {
        isOpening = false;
        resetProgressEvent += () => isOpening = false;
    }

    public async Task OpenBox(bool isIncreasing, float progressSpeed = 0.05f, float progressDecreaseSpeed = 0.1f, float maxProgress = 1f)
    {
        this.isIncreasing = isIncreasing;
        progress = 0;
        openProgress = new OpenProgress(progressSpeed, progressDecreaseSpeed, maxProgress);
        await OnOpen();
    }

    public void InteruptOpening()
    {
        if (!isOpening) return;
        ResetHandler();
    }

    public void StartIncreasingProgress()
    {
        if (!isOpening) return;
        isIncreasing = true;
    }

    public void StartDecreasingProgress()
    {
        if (!isOpening) return;
        isIncreasing = false;
    }

    public event Action<float> onProgressChanged;
    public event Action onOpeningTryEnded;
    public event Action onBoxOpened;
    
    async Task OnOpen()
    {
        if (isOpening) return; 
        isOpening = true;
        Debug.Log("Open performed, starting progress...");
        System.Threading.CancellationToken cts = destroyCancellationToken;
        Func<bool> increasing = () => isIncreasing;
        try
        {
            await openProgress.StartOpening(
                token: cts,
                subscribeReset: (handler) => resetProgressEvent += handler,
                unsubscribeReset: (handler) => resetProgressEvent -= handler,
                increasing: increasing,
                currentProgress: (p) => { progress = p; onProgressChanged?.Invoke(p);},
                onBoxOpened: () => { Debug.Log("Box opened!"); onBoxOpened?.Invoke(); },
                onEnded: () => { Debug.Log("Method ended."); isOpening = false; onOpeningTryEnded?.Invoke(); }
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during opening progress: {ex}");
        }
    }

    void ResetHandler() { resetProgressEvent?.Invoke(); Debug.Log("Reset performed"); progress = 0f; }
    event Action resetProgressEvent;
}

public class OpenProgress
{
    float progress = 0f;
    float progressSpeed = 0.05f;
    float progressDecreaseSpeed = 0.1f;
    float maxProgress = 1f;

    public OpenProgress(float progressSpeed = 0.05f, float progressDecreaseSpeed = 0.1f, float maxProgress = 1f)
    {
        this.progressSpeed = progressSpeed;
        this.progressDecreaseSpeed = progressDecreaseSpeed;
        this.maxProgress = maxProgress;
    }
    
    public float GetProgress() => progress;

    public async Task StartOpening(
        System.Threading.CancellationToken token,
        Action<Action> subscribeReset,
        Action<Action> unsubscribeReset,
        Func<bool> increasing,
        Action<float> currentProgress,
        Action onBoxOpened,
        Action onEnded
    )
    {
        Func<bool> resetCalled = () => false;
        void ResetHandler()
        {
            resetCalled = () => true;
            Debug.Log("Reset called");
        }
        subscribeReset(ResetHandler);
        while (progress >= 0f && progress < maxProgress && !token.IsCancellationRequested && !resetCalled())
        {
            if (increasing())
            {
                progress += progressSpeed * Time.deltaTime;
            }
            else
            {
                progress -= progressDecreaseSpeed * Time.deltaTime;
            }

            currentProgress(progress);

            await Task.Yield();
        }

        unsubscribeReset(ResetHandler);
        Debug.Log("Resetcalled: " + resetCalled());

        if (resetCalled() || progress < 0f)
        {
            progress = 0f;
            onEnded?.Invoke();
            return;
        }

        if (!token.IsCancellationRequested)
        {
            onBoxOpened?.Invoke();
        }
        onEnded?.Invoke();
    }
}