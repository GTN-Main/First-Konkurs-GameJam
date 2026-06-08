using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPresser : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Image buttonImage;

    [SerializeField]
    Color normalColor = Color.white;

    [SerializeField]
    Color pressedColor = Color.gray;
    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = pressedColor;

        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;

        isPressed = false;
    }

    public bool IsPressed()
    {
        return isPressed;
    }
}
