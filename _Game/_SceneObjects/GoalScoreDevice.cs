﻿using Assets.Scripts;
using AutoLevelMenu;
using AutoLevelMenu.Enums;
using AutoLevelMenu.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.ValidationSuite;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
/// </summary>
/// 
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
    private GameEvent goalEvent;


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

    public void Respawn()
    {
        SetVisibleAndCollidable(true, true);
        // Set back to original location.
        transform.SetPositionAndRotation(originalLocation, originalRotation);
        //rb.isKinematic = false;
        // Reset all physics
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Reset values on respawn
        RestValuesOnRespawn();
    }

    public void StartMoving()
    {
        UnFreeze();
    }

    class PlayerHitData
    {
        public int TimeTouched { get; set; }
        public Player Player { get; set; }
        public PlayerSpar PlayerSpar { get; set; }
    }

    // TODO make these values actually reset on spawn. This does not happen atm.
    class ResetableValuesOnRespawn
    {
        public bool HasScored { get; set; } = false;
        //public Dictionary<Player, int> PlayerTimeTouched { get; set; } = new Dictionary<Player, int>();
        public List<PlayerHitData> PlayerHitDatas = new();

        /// <summary>
        /// Gets last hit player data for player not on the team of the goal that was scored or player of the goal that was scored.
        /// </summary>
        /// <param name="team"></param>
        public PlayerHitData GetLastHitPlayerData(int? goalTeam, int? playerNumber)
        {
            if (goalTeam != null)
            {
                return PlayerHitDatas.Where(ph => ph.PlayerSpar.Team != goalTeam).LastOrDefault();
            }
            if (playerNumber != null)
            {
                return PlayerHitDatas.Where(ph => ph.Player.PlayerNumber != playerNumber).LastOrDefault();
            }
            // Just return the last hit player data
            return PlayerHitDatas.LastOrDefault();
        }
    }

    ResetableValuesOnRespawn ResetValuesOnGoal { get; set; } = new ResetableValuesOnRespawn();

    [Serializable]
    public class CollisionEnterTypes
    {
        public CollisionEnterType teamGoal;
        public CollisionEnterType playerGoal;
    }
    public CollisionEnterTypes collisionEnterTypes;

    [SerializeField]
    PlayerScoreManager playerScoreManager;

    [SerializeField]
    PlayerSparData playerSparData;

    void OnCollisionEnter(Collision collision)
    {
        RecordPlayerHits(collision.transform);
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log($"Checking Trigger{other.GetComponent<Transform>()?.name}" ?? "null");
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
        //Debug.Log($"{other.name} is completely inside {name}");
        EnumCollision(other.transform);
    }

    void RecordPlayerHits(Transform other)
    {
        if (!other.TryGetComponent<PlayerObject>(out var playerObject)) return;
        ResetValuesOnGoal.PlayerHitDatas.Add(new PlayerHitData()
        {
            TimeTouched = FixedUpdateClock.Clock,
            Player = playerObject.Player,
            PlayerSpar = playerObject.PlayerSpar,
        });
    }

    // Use Respawn Event to call this
    public void RestValuesOnRespawn()
    {
        ResetValuesOnGoal = new ResetableValuesOnRespawn();
    }

    void EnumCollision(Transform other)
    {
        if (!other.gameObject.TryGetComponent<CollisionEnter>(out var collisionEnter)) return;
        var collisionEnterType = collisionEnter.collisionEnterType;
        // Goal
        var teamGoal = collisionEnterType == collisionEnterTypes.teamGoal;
        var playerGoal = collisionEnterType == collisionEnterTypes.playerGoal;
        if ((teamGoal || playerGoal) && !ResetValuesOnGoal.HasScored)
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
        // Raise goal event
        goalEvent.Raise();

        ResetValuesOnGoal.HasScored = true;
        HandleGoalScorePoint(playerGoal: playerGoal, teamGoal: teamGoal, collisionEnter: collisionEnter);

        // Start respawn timer
        Freeze();
        SetVisibleAndCollidable(false, false);
    }

    private void HandleGoalScorePoint(bool teamGoal, bool playerGoal, CollisionEnter collisionEnter)
    {
        // Add to score and count who last hit it
        (var teamScoredOn, var playerScoredOn) = FuncUtil.Invoke<(int?, int?)>(() =>
        {
            if (teamGoal)
                return (collisionEnter.intField, null);
            if (playerGoal)
                return (null, collisionEnter.intField);
            throw new Exception("Should be either team or player goal");
        });
        // Send out event that team was scored on
        var lastHitPlayerData = ResetValuesOnGoal.GetLastHitPlayerData(teamScoredOn, playerScoredOn);
        Debug.Log($"Player {lastHitPlayerData?.Player.PlayerName} on {lastHitPlayerData?.PlayerSpar.Team} team scored on {teamScoredOn}");

        // If someone scored then they/their team gets the point.
        if (lastHitPlayerData != null)
        {
            playerScoreManager.AddScore(lastHitPlayerData.PlayerSpar.Team, lastHitPlayerData.Player.PlayerNumber);
        }
        else
        {
            // If no one scored then everyone but the team/player gets a point.
            // Add to every other team
            var allTeams = playerSparData.AllTeams();
            if (allTeams.Count > 0)
            {
                var teams = allTeams.Where(t => t != teamScoredOn).ToList();
                teams.ForEach(t => playerScoreManager.AddScore(t, 0 /* Set to 0 because it does not matter as long as t is not 0 */));
            }

            // Add to every other player
            var singlePlayers = playerSparData.SinglePlayers().Where(p => p != playerScoredOn).ToList();
            singlePlayers.ForEach(sp => playerScoreManager.AddScore(0, sp));
        }
    }
}
