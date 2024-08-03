public class ShowScoresState : IState<TeamScoreGameEnd>
{
    public void Enter(TeamScoreGameEnd context)
    {
        // The ending scores pop up: players can't move - 3 seconds
        // Implement logic to display scores
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to WaitForInputState after some condition, e.g., a timer
        context.StateMachine.ChangeState(new WaitForInputState());
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for ShowScores state
    }
}
