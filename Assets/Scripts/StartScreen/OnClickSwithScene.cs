using UnityEngine;
using UnityEngine.InputSystem;

public class OnClickSwithScene : MonoBehaviour
{
    void Start()
    {
        var clickAction = InputManager.Instance.CurrentMap_global.FindAction("Click");
        if (clickAction != null)
        {
            clickAction.performed += OnMouseDown;
        }
    }
    
    void OnMouseDown(InputAction.CallbackContext context)
    {
        var clickAction = InputManager.Instance.CurrentMap_global.FindAction("Click");
        if (clickAction != null)
        {
            clickAction.performed -= OnMouseDown;
        }
        GameManager.Instance.ChangeGameState(GameManager.GameStateTag.StartGame);
    }
}
