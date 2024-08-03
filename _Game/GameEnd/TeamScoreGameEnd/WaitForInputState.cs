public class WaitForInputState : IState<TeamScoreGameEnd>
{
    public void Enter(TeamScoreGameEnd context)
    {
        // Press A to continue: now you can leave the game
        // Implement logic to wait for player input
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Wait for player input and transition to the appropriate state
        // For example, reset the game or exit to main menu
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for WaitForInput state
    }
}
