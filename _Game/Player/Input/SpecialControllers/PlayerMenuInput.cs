using AutoLevelMenu.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuInput : PlayerInputBase
{
    public GameEvent startPressedEvent;
    public GameEvent leftShoulderPressedEvent;
    public GameEvent rightShoulderPressedEvent;
    public GameObject mainMenu;
    Player player;
    PlayerSpar playerSpar;

    private void Start()
    {
        player = PlayerInputTransform.GetComponent<Player>();
        playerSpar = PlayerInputTransform.GetComponent<PlayerSpar>();
    }

    public override void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
        if (inputAction.action.phase == InputActionPhase.Performed)
        {
            switch (inputAction.action.name)
            {
                case PlayerInputs.LeftShoulder:
                    // show chosen team
                    leftShoulderPressedEvent.Raise();
                    if (mainMenu.activeSelf) return;
                    playerSpar.ChoosenTeam--;
                    Debug.Log(playerSpar.ChoosenTeam);
                    break;
                case PlayerInputs.RightShoulder:
                    // Change chosen team
                    rightShoulderPressedEvent.Raise();
                    if (mainMenu.activeSelf) return;
                    playerSpar.ChoosenTeam++;
                    Debug.Log(playerSpar.ChoosenTeam);
                    break;
                case PlayerInputs.Start:
                    startPressedEvent.Raise();
                    break;
                case PlayerInputs.ButtonNorth:
                    player.Spawn();
                    break;
            }
        }
    }
}
