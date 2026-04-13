using TMPro;
using UnityEngine;

public class BoxUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject targetCanvas;
    public GameObject GetTargetCanvas() => targetCanvas;

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
