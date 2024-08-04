
public class FreezePlayersState : IState<TeamScoreGameEnd>
{
    public CountDownTimer countDownTimer = new(100);

    public void Enter(TeamScoreGameEnd context)
    {
        // Freeze all of the rigidbodies for a moment
        // Implement logic to freeze players
        //context.playerSparData.Players.ForEach(player =>
        //{
        //    player.DestroyAllPlayerControlledObjects();
        //});
        context.teamPlayerManager.FreezeAllPlayers();
        countDownTimer.StartTimer();
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to ShowGameOverState after some condition, e.g., a timer
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
        countDownTimer.CountDown();
        if (countDownTimer.IsFinished())
        {
            context.StateMachine.ChangeState(new ShowGameOverState());
        }
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for FreezePlayers state
    }
}
