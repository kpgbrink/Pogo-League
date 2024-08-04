using UnityEngine;

public class ShowGameOverState : IState<TeamScoreGameEnd>
{
    public CountDownTimer countDownTimer = new(200);
    public void Enter(TeamScoreGameEnd context)
    {
        // Show GAME OVER -- 3 seconds
        // Implement logic to display "Game Over"
        context.gameOverText.SetActive(true);
        Debug.Log("ShowGameOverState");
        countDownTimer.StartTimer();
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to AllowWinningTeamMovementState after some condition, e.g., a timer
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
        countDownTimer.CountDown();
        if (countDownTimer.IsFinished())
        {
            context.StateMachine.ChangeState(new AllowWinningTeamMovementState());
        }
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for ShowGameOver state
        context.gameOverText.SetActive(false);
    }
}
