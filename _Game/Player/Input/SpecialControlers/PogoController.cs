using AutoLevelMenu.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PogoController : PlayerInputBase
{
    public float moveSpeed = 10f;
    Vector2 inputMovement;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.AddForce(inputMovement * moveSpeed);
    }

    public override void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
        if (inputAction.action.phase == InputActionPhase.Performed)
        {
            switch (inputAction.action.name)
            {
                case PlayerInputs.LeftStick:
                    // Go backwards
                    inputMovement = inputAction.action.ReadValue<Vector2>();
                    break;
            }
        }
    }
}
