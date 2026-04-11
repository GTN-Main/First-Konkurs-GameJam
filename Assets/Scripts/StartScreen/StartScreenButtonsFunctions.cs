using UnityEngine;

public class StartScreenButtonsFunctions : MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        Debug.Log("Play button clicked!");
        GameManager.Instance.ChangeGameState(GameManager.GameStateTag.StartGame);
    }

    public void OnSettingsButtonClick()
    {
        // Implement settings functionality
        Debug.Log("Settings button clicked!");
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
        Debug.Log("Quit button clicked!");
    }
}
