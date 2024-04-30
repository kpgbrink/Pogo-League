using Assets.Scripts;
using AutoLevelMenu.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GoalScoreDevice : MonoBehaviour
{
    public FixedUpdateClock FixedUpdateClock => GameObject.Find("/FixedUpdateClock").GetComponent<FixedUpdateClock>();

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
        public List<PlayerHitData> PlayerHitDatas = new List<PlayerHitData>();

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
        EnumCollision(collision.transform);
    }

    void OnTriggerEnter(Collider other)
    {
        RecordPlayerHits(other.transform);
        EnumCollision(other.transform);
    }

    void RecordPlayerHits(Transform other)
    {
        var playerObject = other.GetComponent<PlayerObject>();
        if (playerObject == null) return;
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
        var collisionEnter = other.gameObject.GetComponent<CollisionEnter>();
        if (collisionEnter == null) return;
        var collisionEnterType = collisionEnter.collisionEnterType;
        // Goal
        var teamGoal = collisionEnterType == collisionEnterTypes.teamGoal;
        var playerGoal = collisionEnterType == collisionEnterTypes.playerGoal;
        if (teamGoal || playerGoal && !ResetValuesOnGoal.HasScored)
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
            Debug.Log($"Player {lastHitPlayerData?.Player.PlayerName} on {lastHitPlayerData?.PlayerSpar.Team} team scored on { teamScoredOn}");

            // If someone scored then they/their team gets the point.
            if (lastHitPlayerData != null)
            {
                playerScoreManager.AddScore(lastHitPlayerData.PlayerSpar.Team, lastHitPlayerData.Player.PlayerNumber);
            } else
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
        ResetValuesOnGoal.HasScored = true;
    }
}
