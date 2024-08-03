
public class FreezePlayersState : IState<TeamScoreGameEnd>
{
    CountDownTimer freezeDurationTimer;

    public void Enter(TeamScoreGameEnd context)
    {
        // Freeze all of the rigidbodies for a moment
        // Implement logic to freeze players
        //context.playerSparData.Players.ForEach(player =>
        //{
        //    player.DestroyAllPlayerControlledObjects();
        //});
        context.teamPlayerManager.FreezeAllPlayers();
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to ShowGameOverState after some condition, e.g., a timer
        context.StateMachine.ChangeState(new ShowGameOverState());
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for FreezePlayers state
    }
}
