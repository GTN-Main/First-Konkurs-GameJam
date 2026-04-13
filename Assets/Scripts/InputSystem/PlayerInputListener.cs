using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerTag { Player1, Player2 }
public class PlayerInputListener : MonoBehaviour
{
    public static PlayerInputListener Instance { get; private set; }

    Dictionary<PlayerTag, PlayerInputData> playerInputs = new Dictionary<PlayerTag, PlayerInputData>()
    {
        { PlayerTag.Player1, new PlayerInputData() },
        { PlayerTag.Player2, new PlayerInputData() }
    };

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var playerTag in playerInputs.Keys)
        {
            var actionMap = GetActionsForPlayer(playerTag);
            playerInputs[playerTag].SetActionMap(actionMap);
        }
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        foreach (var playerTag in playerInputs.Keys)
        {
            playerInputs[playerTag].SetActionMap(null);
        }
    }

    public PlayerInputData GetPlayerInput(PlayerTag playerTag)
    {
        PlayerInputData inputData = null;
        if (playerInputs.ContainsKey(playerTag))
        {
            inputData = playerInputs[playerTag];
        }
        return inputData;
    }

    InputActionMap GetActionsForPlayer(PlayerTag playerTag)
    {
        switch (playerTag)
        {
            case PlayerTag.Player1:
                return InputManager.Instance?.CurrentMap_player1;
            case PlayerTag.Player2:
                return InputManager.Instance?.CurrentMap_player2;
        }
        return null;
    }
}

[Serializable]
public class PlayerInputData
{
    InputActionMap _action;
    InputAction move_action;
    InputAction interact_action;
    InputAction attack_action;
    public void SetActionMap(InputActionMap Action)
    {
        _action = Action;
        move_action = _action?.FindAction("Move");
        interact_action = _action?.FindAction("Interact");
        attack_action = _action?.FindAction("Attack");
    }


    #region Movement properties
    public bool up => move_action != null ? move_action.ReadValue<Vector2>().y > 0 : false;
    public bool right => move_action != null ? move_action.ReadValue<Vector2>().x > 0 : false;
    public bool down => move_action != null ? move_action.ReadValue<Vector2>().y < 0 : false;
    public bool left => move_action != null ? move_action.ReadValue<Vector2>().x < 0 : false;
    public Vector2 direction => move_action != null ? move_action.ReadValue<Vector2>() : Vector2.zero;
    #endregion

    #region Interaction properties
    public bool interact => interact_action != null ? interact_action.IsPressed() : false;
    #endregion

    #region Attack properties
    public bool attack => attack_action != null ? attack_action.IsPressed() : false;
    #endregion
}
