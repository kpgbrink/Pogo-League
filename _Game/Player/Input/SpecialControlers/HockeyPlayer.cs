using AutoLevelMenu.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HockeyPlayer : PlayerInputBase
{
    [SerializeField]
    float moveSpeed = 5f;

    [SerializeField]
    CountDownTimer speedBoostTimer;

    Vector2 leftStick;
    Vector2 rightStick;
    Rigidbody mainBodyRb;

    // Start is called before the first frame update
    void Start()
    {
        mainBodyRb = transform.Find("MainBody").GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        FixedUpdateInputHandling();
    }


    private void FixedUpdateInputHandling()
    {
        if (!ControlActive) return;
        mainBodyRb.AddForce(new Vector3(leftStick.x, 0, leftStick.y) * moveSpeed);
    }

    public override void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
        if (!ControlActive) return;
        if (inputAction.action.phase == InputActionPhase.Performed)
        {
            switch (inputAction.action.name)
            {
                case PlayerInputs.LeftStick:
                    // Go backwards
                    leftStick = inputAction.action.ReadValue<Vector2>();
                    //Debug.Log("LeftStick moved" + leftStick);
                    break;
                case PlayerInputs.RightStick:
                    // Go through forwards
                    rightStick = inputAction.action.ReadValue<Vector2>();
                    break;
                case PlayerInputs.LeftShoulder:
                case PlayerInputs.RightShoulder:
                    mainBodyRb.linearVelocity = Vector3.zero;
                    break;
                case PlayerInputs.LeftTrigger:
                case PlayerInputs.RightTrigger:
                    if (speedBoostTimer.CheckFinished(true))
                    {
                        mainBodyRb.linearVelocity *= 2;
                    }
                    break;
            }
        }
    }
}
