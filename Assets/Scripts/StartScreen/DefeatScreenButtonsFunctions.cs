using UnityEngine;

public class DefeatScreenButtonsFunctions : MonoBehaviour
{
    public void OnBackButtonClick()
    {
        Debug.Log("Back button clicked!");
        GameManager.Instance.BackToStartScreen();
    }
}
