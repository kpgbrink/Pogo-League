
using UnityEngine;


public class AllowWinningTeamMovementState : IState<TeamScoreGameEnd>
{
    public CountDownTimer countDownTimer = new(0);
    public void Enter(TeamScoreGameEnd context)
    {
        // Blue/Red team wins: The team that wins can move for 3 seconds - 5 seconds
        // Implement logic to allow winning team movement

        countDownTimer.StartTimer();

        // UnFreeze the winners
        context.teamPlayerManager.UnFreezeSomePlayers(context.PlayerSparsWinner);
        Debug.Log("Allow some to move");
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to ShowScoresState after some condition, e.g., a timer
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
        countDownTimer.CountDown();
        if (countDownTimer.IsFinished())
        {
            context.StateMachine.ChangeState(new ShowWhoOneState());
        }
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for AllowWinningTeamMovement state
    }
}