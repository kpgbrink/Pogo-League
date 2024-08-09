using Assets.Scripts;
using AutoLevelMenu.Enums;
using AutoLevelMenu.Events;
using System;
using System.Linq;
using UnityEngine;

public class GoalScoreDeviceGameManager : MonoBehaviour
{
    [SerializeField]
    ScoreManager scoreManager;

    [SerializeField]
    GameEvent startGameCountdownTimer;

    [SerializeField]
    GameEvent ballTouchedGroundOrScoredToEndGame;

    [SerializeField]
    GameEvent ballScoredInOvertimeToEndGame;

    [SerializeField]
    ScoreManager playerScoreManager;

    [NonSerialized]
    public bool waitingForBallTouchGroundOrScoredToEndGame = false;
    bool waitingForBallToScoreToEndGame = false;


    [SerializeField]
    PlayerSparData playerSparData;


    [SerializeField]
    private GameEvent goalEvent;

    public bool HasStartedCountdownTimer { get; set; } = false;

    public ScoreManager ScoreManager => scoreManager;

    public ResetableValuesOnRespawn resetValuesOnGoal = new();

    public void OnStartOvertime()
    {
        waitingForBallToScoreToEndGame = true;
    }

    public void OnStartWaitingForBallToTouchGroundOrScoredToEndGame()
    {
        waitingForBallTouchGroundOrScoredToEndGame = true;
    }

    public void StartOvertime()
    {
        waitingForBallToScoreToEndGame = true;
        // Additional logic to start overtime
    }

    public void StartWaitingForBallToTouchGroundOrScoredToEndGame()
    {
        waitingForBallTouchGroundOrScoredToEndGame = true;
    }

    public void GoalScored(CollisionEnter collisionEnter)
    {
        //scoreManager.RecordGoal(collisionEnter);

        if (waitingForBallTouchGroundOrScoredToEndGame)
        {
            waitingForBallTouchGroundOrScoredToEndGame = false;
            ballTouchedGroundOrScoredToEndGame.Raise();
        }
        if (waitingForBallToScoreToEndGame)
        {
            waitingForBallToScoreToEndGame = false;
            ballScoredInOvertimeToEndGame.Raise();
        }

        resetValuesOnGoal.HasScored = true;
    }

    public void HandleStartCountdownCheck(PlayerObject playerObject)
    {
        if (!HasStartedCountdownTimer)
        {
            HasStartedCountdownTimer = true;
            startGameCountdownTimer.Raise();
        }
    }

    public void ResetValuesOnRespawn()
    {
        HasStartedCountdownTimer = false;
        resetValuesOnGoal = new ResetableValuesOnRespawn();
    }

    public void RaiseBallTouchedGroundOrScoredToEndGameEvent()
    {
        ballTouchedGroundOrScoredToEndGame.Raise();
    }

    public class ResetableValuesOnRespawn
    {
        public bool HasStartedCountdownTimer { get; set; } = false;
        public bool HasScored { get; set; } = false;
    }

    public void GoalScored(bool playerGoal, bool teamGoal, CollisionEnter collisionEnter)
    {
        if (!waitingForBallTouchGroundOrScoredToEndGame && !waitingForBallToScoreToEndGame)
        {
            // Raise goal event
            goalEvent.Raise();
        }

        resetValuesOnGoal.HasScored = true;
        HandleGoalScorePoint(playerGoal: playerGoal, teamGoal: teamGoal, collisionEnter: collisionEnter);


        // If the ball touched ground to end the game is active then need to check if the game has ended. And if it has not then need to start the overtime.
        // This has to happen at the end because the score needs to be counted before you can check if the game has ended.
        if (waitingForBallTouchGroundOrScoredToEndGame)
        {
            waitingForBallTouchGroundOrScoredToEndGame = false;
            ballTouchedGroundOrScoredToEndGame.Raise();
        }
        if (waitingForBallToScoreToEndGame)
        {
            ballScoredInOvertimeToEndGame.Raise();
        }
    }

    void HandleGoalScorePoint(bool teamGoal, bool playerGoal, CollisionEnter collisionEnter)
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