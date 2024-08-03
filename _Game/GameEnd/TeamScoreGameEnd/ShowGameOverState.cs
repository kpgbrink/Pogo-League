public class ShowGameOverState : IState<TeamScoreGameEnd>
{
    public void Enter(TeamScoreGameEnd context)
    {
        // Show GAME OVER -- 3 seconds
        // Implement logic to display "Game Over"
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to AllowWinningTeamMovementState after some condition, e.g., a timer
        context.StateMachine.ChangeState(new AllowWinningTeamMovementState());
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for ShowGameOver state
    }
}
