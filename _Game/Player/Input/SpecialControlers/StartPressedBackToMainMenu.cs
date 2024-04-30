using AutoLevelMenu.Common;
using AutoLevelMenu.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartPressedBackToMainMenu : PlayerInputBase
{
    [SerializeField]
    LevelsData LevelsData;

    [SerializeField]
    StringGameEvent normalLoadEvent;

    public override void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
        if (inputAction.action.phase == InputActionPhase.Performed)
        {
            switch (inputAction.action.name)
            {
                case PlayerInputs.StartHeld:
                    // Go through forwards
                    normalLoadEvent.Raise(LevelsData.SceneLocationFullPath(LevelsData.sceneLocations.mainMenu));
                    break;
            }
        }
    }
}
