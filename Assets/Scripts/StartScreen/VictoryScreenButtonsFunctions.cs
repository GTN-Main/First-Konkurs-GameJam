using UnityEngine;

public class VictoryScreenButtonsFunctions : MonoBehaviour
{
    public void OnBackButtonClick()
    {
        Debug.Log("Back button clicked!");
        GameManager.Instance.BackToStartScreen();
    }
}
