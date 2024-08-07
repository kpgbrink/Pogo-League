using AutoLevelMenu.Enums;
using AutoLevelMenu.Events;
using UnityEngine;

public class GoalScoreDeviceGameManager : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager;

    [SerializeField]
    private GameEvent startGameCountdownTimer;

    [SerializeField]
    private GameEvent ballTouchedGroundOrScoredToEndGame;

    [SerializeField]
    private GameEvent ballScoredInOvertimeToEndGame;

    private bool waitingForBallTouchGroundOrScoredToEndGame = false;
    private bool waitingForBallToScoreToEndGame = false;

    public ScoreManager ScoreManager => scoreManager;

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
    }

    public void HandleStartCountdownCheck(PlayerObject playerObject)
    {
        //if (!scoreManager.HasStartedCountdownTimer)
        {
            //scoreManager.HasStartedCountdownTimer = true;
            startGameCountdownTimer.Raise();
        }
    }
}
