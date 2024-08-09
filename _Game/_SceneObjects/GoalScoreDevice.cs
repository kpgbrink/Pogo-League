using AutoLevelMenu;
using AutoLevelMenu.Enums;
using AutoLevelMenu.Events;
using System;
using UnityEngine;

/// <summary>
/// 
/// </summary>
// Require rigidbody to be able to reset physics
[RequireComponent(typeof(Rigidbody))]
public class GoalScoreDevice : MonoBehaviour
{
    // Store original location
    private Vector3 originalLocation;

    // Store original rotation
    private Quaternion originalRotation;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    FixedUpdateClock FixedUpdateClock;

    [SerializeField]
    GameObject defaultExplosionPrefab;

    GameObject defaultExplosionInstance;

    [SerializeField]
    MeshRenderer meshRenderer;

    [SerializeField]
    SphereCollider SphereCollider;

    [SerializeField]
    private Vector3FloatGameEvent explosionEvent;

    [SerializeField]
    private GameEvent startGameCountdownTimer;

    [SerializeField]
    private GameEvent ballTouchedGroundOrScoredToEndGame;

    [SerializeField]
    private GameEvent ballScoredInOvertimeToEndGame;

    [SerializeField]
    GoalScoreDeviceGameManager goalScoreDeviceGameManager;


    [Serializable]
    public class CollisionEnterTypes
    {
        public CollisionEnterType teamGoal;
        public CollisionEnterType playerGoal;
        public CollisionEnterType ground;
    }
    public CollisionEnterTypes collisionEnterTypes;

    /// <summary>
    /// This is test to get the variable I would have to change for the stats. One example is the goals
    /// </summary>
    public void PlayerSparTest()
    {
        playerSparData.PlayerSpars.ForEach(ps =>
        {
            Debug.Log(ps.ResetValues.Goals);
        });
        playerSparData.Players.ForEach(p =>
        {
            p.playerSpar.ResetValues.Goals = 0;
        });
    }

    public void OnStartOvertime()
    {
        // Freeze the ball
        Freeze();
    }

    void CheckBallTouchGroundOnWaitingToEndGame(Collision other)
    {
        if (!goalScoreDeviceGameManager.waitingForBallTouchGroundOrScoredToEndGame)  // Check the state from BallGameManager
        {
            return;
        }
        if (!other.gameObject.TryGetComponent<CollisionEnter>(out var collisionEnter)) return;
        var collisionEnterType = collisionEnter.collisionEnterType;
        if (collisionEnterType == collisionEnterTypes.ground)  // Use CollisionEnterTypes from BallGameManager
        {
            Freeze();
            goalScoreDeviceGameManager.waitingForBallTouchGroundOrScoredToEndGame = false;  // Update state in BallGameManager
            goalScoreDeviceGameManager.RaiseBallTouchedGroundOrScoredToEndGameEvent();  // Raise the event from BallGameManager
        }
    }

    private void Start()
    {
        StoreOriginalPosition();
        InstantiateDefaultExplosion();
        Freeze();
    }

    void Freeze()
    {
        rb.isKinematic = true;
    }

    void UnFreeze()
    {
        rb.isKinematic = false;
    }

    void InstantiateDefaultExplosion()
    {
        defaultExplosionInstance = Instantiate(defaultExplosionPrefab, transform.position, transform.rotation);
        defaultExplosionInstance.SetActive(false);
    }

    void TriggerExplosion(Vector3 position, Quaternion rotation, float goalSpeed)
    {
        defaultExplosionInstance.transform.SetPositionAndRotation(position, rotation);
        defaultExplosionInstance.SetActive(true);
        if (defaultExplosionInstance.TryGetComponent<ExplosionController>(out var explosionController))
        {
            explosionController.ActivateExplosion(position, rotation, goalSpeed);
        }
    }

    void BlastPlayers(Vector3 position, float goalSpeed)
    {
        // Trigger explosion event
        explosionEvent.Raise(position, goalSpeed * 25f); // Adjust force as needed
    }

    private void StoreOriginalPosition()
    {
        originalLocation = transform.position;
        originalRotation = transform.rotation;
    }

    void SetVisibleAndCollidable(bool visible, bool collidable)
    {
        meshRenderer.enabled = visible;
        SphereCollider.enabled = collidable;
    }

    public void OnRespawn()
    {
        SetVisibleAndCollidable(true, true);
        // Set back to original location.
        transform.SetPositionAndRotation(originalLocation, originalRotation);
        //rb.isKinematic = false;
        // Reset all physics
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Reset values on respawn
        goalScoreDeviceGameManager.ResetValuesOnRespawn();
    }

    public void OnStartMoving()
    {
        UnFreeze();
    }

    [SerializeField]
    PlayerSparData playerSparData;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent<PlayerObject>(out var playerObject))
        {
            goalScoreDeviceGameManager.HandleStartCountdownCheck(playerObject);
        }
        CheckBallTouchGroundOnWaitingToEndGame(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckBallTouchGroundOnWaitingToEndGame(collision);
    }

    void OnTriggerStay(Collider other)
    {
        var boxCollider = other as BoxCollider;
        var sphereCollider = GetComponent<SphereCollider>();

        if (sphereCollider != null && boxCollider != null)
        {
            if (Utils.CheckSphereCompletelyInsideBox(sphereCollider, boxCollider))
            {
                HandleCompleteOverlap(other);
            }
        }
    }

    private void HandleCompleteOverlap(Collider other)
    {
        GoalScore(other.transform);
    }

    void GoalScore(Transform other)
    {
        if (!other.gameObject.TryGetComponent<CollisionEnter>(out var collisionEnter)) return;
        var collisionEnterType = collisionEnter.collisionEnterType;
        // Goal
        var teamGoal = collisionEnterType == collisionEnterTypes.teamGoal;
        var playerGoal = collisionEnterType == collisionEnterTypes.playerGoal;
        if ((teamGoal || playerGoal) && !goalScoreDeviceGameManager.resetValuesOnGoal.HasScored)
        {
            GoalScored(playerGoal: playerGoal, teamGoal: teamGoal, collisionEnter: collisionEnter);
        }
    }

    void GoalScored(bool playerGoal, bool teamGoal, CollisionEnter collisionEnter)
    {
        // Trigger explosion
        TriggerExplosion(transform.position, transform.rotation, rb.linearVelocity.magnitude);
        // Blast players away from the explosion
        BlastPlayers(transform.position, rb.linearVelocity.magnitude);

        // Start respawn timer
        Freeze();
        SetVisibleAndCollidable(false, false);
        goalScoreDeviceGameManager.GoalScored(playerGoal, teamGoal, collisionEnter);
    }
}
