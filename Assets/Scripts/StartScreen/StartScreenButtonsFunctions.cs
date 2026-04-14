using UnityEngine;

public class StartScreenButtonsFunctions : MonoBehaviour
{
    bool rulesPanelActive;
    [SerializeField] GameObject rulesPanel;
    [SerializeField] GameObject startScreenPanel;

    public void Start()
    {
        if (rulesPanel == null)
            Debug.LogError("Rules panel reference is not set in the inspector!");
        if (startScreenPanel == null)
            Debug.LogError("Start screen panel reference is not set in the inspector!");

        rulesPanelActive = false;
        UpdateCanvas();
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("Play button clicked!");
        GameManager.Instance.StartGame();
    }

    public void OnRulesButtonClick()
    {
        // Implement rules functionality
        Debug.Log("Rules button clicked!");
        rulesPanelActive = true;
        UpdateCanvas();
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
        Debug.Log("Quit button clicked!");
    }

    public void OnRulesOkButtonClick()
    {
        // Implement rules OK functionality
        Debug.Log("Rules OK button clicked!");
        rulesPanelActive = false;
        UpdateCanvas();
    }

    void UpdateCanvas()
    {
        if (rulesPanel != null)
            rulesPanel.SetActive(rulesPanelActive);
        if (startScreenPanel != null)
            startScreenPanel.SetActive(!rulesPanelActive);
    }
}
