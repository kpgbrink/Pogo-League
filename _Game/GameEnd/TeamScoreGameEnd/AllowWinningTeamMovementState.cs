public class AllowWinningTeamMovementState : IState<TeamScoreGameEnd>
{
    public void Enter(TeamScoreGameEnd context)
    {
        // Blue/Red team wins: The team that wins can move for 3 seconds - 5 seconds
        // Implement logic to allow winning team movement
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to ShowScoresState after some condition, e.g., a timer
        context.StateMachine.ChangeState(new ShowScoresState());
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for AllowWinningTeamMovement state
    }
}