using UnityEngine;
using UnityEngine.InputSystem;
using AutoLevelMenu;
using System.Collections.Generic;

public class PogoController : PlayerInputBase
{
    [SerializeField]
    HingeJoint hinge;

    [SerializeField]
    ConfigurableJoint configurableJoint;

    [SerializeField]
    float springForce = 10000f;  // Adjustable spring force
    [SerializeField]
    float damper = 50f;         // Adjustable damping

    private float maxExtension = -2.4f; // Maximum extension of the joint
    [SerializeField]
    private float contractionForce = 10000f; // Force to apply for contraction

    Vector2 inputMovement;

    bool IsWaitingForKickoff = false;

    [SerializeField]
    GameStartCountDown gameStartCountDown;

    [SerializeField]
    FixedUpdateClock fixedUpdateClock;

    private Dictionary<Rigidbody, RigidbodyConstraints> originalConstraints = new Dictionary<Rigidbody, RigidbodyConstraints>();

    public void Start()
    {
        Debug.Log(configurableJoint);

        // A player could spawn during countdown, so we need to wait for kickoff if it in that setting.
        // This will make the player wait for kickoff. But also freeze the player before the hmm
        if (gameStartCountDown != null && !gameStartCountDown.GetCountDownTimerFinished())
        {
            Debug.Log("Freeeeeeeeeeeeeeeze");
            WaitForKickOff();
        }
    }

    public void WaitForKickOff()
    {
        // only respawn if the game has not started
        if (fixedUpdateClock != null && !fixedUpdateClock.ResetValuesOnSceneInit.GameBeforeStart)
        {
            Respawn();
        }
        inputMovement = Vector2.zero;
        IsWaitingForKickoff = true;
        FreezeRigidBodies();
    }

    public void KickOff()
    {
        IsWaitingForKickoff = false;
        UnFreezeRigidBodies();
    }

    void FreezeRigidBodies()
    {
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (!originalConstraints.ContainsKey(rb))
            {
                originalConstraints[rb] = rb.constraints;
            }
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void UnFreezeRigidBodies()
    {
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            if (originalConstraints.ContainsKey(rb))
            {
                rb.constraints = originalConstraints[rb];
            }
        }
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

    void FixedUpdate()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (IsWaitingForKickoff) { return; }
        HandleJoystickInput();
    }


    void HandleJoystickInput()
    {
        // Get the horizontal and vertical values of the left stick
        //Debug.Log($"inputMovment.x: {inputMovement.x} inputMovement.y {inputMovement.y}");
        var horizontalInput = -inputMovement.x;
        var verticalInput = inputMovement.y;
        var inputMagnitude = Mathf.Sqrt(horizontalInput * horizontalInput + verticalInput * verticalInput);

        // Calculate the target angle from stick input
        var rawAngle = Mathf.Atan2(verticalInput, horizontalInput) * Mathf.Rad2Deg + 90;


        // Apply the calculated angle directly to the hinge
        if (hinge != null && !(horizontalInput == 0 && verticalInput == 0))
        {
            var spring = hinge.spring;
            //spring.targetPosition = currentAngle + deltaAngle; // Directly set target position
            spring.targetPosition = Utils.NormalizeAngleTo180(rawAngle); // Directly set target position
            spring.spring = springForce;   // Set spring force dynamically
            spring.damper = damper;        // Set damper dynamically
            hinge.spring = spring;
            hinge.useSpring = true;        // Ensure the spring functionality is enabled
        }

        // Handle extension/contraction via ConfigurableJoint
        //Adjust the target position based on input magnitude
        var drive = configurableJoint.yDrive; // Assuming X-axis for sliding, adjust as needed
        drive.positionSpring = contractionForce; // Use contraction force to push/pull
        drive.positionDamper = 50f; // Damping for smooth movement
        configurableJoint.yDrive = drive;

        //Set the target position relative to the maximum extension
        configurableJoint.targetPosition = new Vector3(0, maxExtension * inputMagnitude, 0); // Modify based on the axis
    }
}
