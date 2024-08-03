using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HumanController : PlayerInputBase
{
    public float moveSpeed = 5f;

    Vector2 inputMovement;
    Vector2 rightInputMovement;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.AddForce(inputMovement * moveSpeed / 2);
        rb.AddForce(rightInputMovement * moveSpeed / 2);
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
                case PlayerInputs.RightStick:
                    // Go through forwards
                    rightInputMovement = inputAction.action.ReadValue<Vector2>();
                    break;
                case PlayerInputs.ButtonWest:
                    // Go through forwards
                    Debug.Log("I destroyed myself");
                    Object.Destroy(this.gameObject);
                    break;
            }
        }
    }
}
