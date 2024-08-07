public class ShowScoresState : IState<TeamScoreGameEnd>
{
    /// <summary>
    /// This is where we show the scores of all of the players. Show who did what.
    /// 
    /// Goals: 
    /// Assists:
    /// Saves: -- this is hard figure it out later
    /// Shots: -- this will be hard figure it out later
    /// Ball Touches:
    /// Average distance from ball:
    /// </summary>
    /// <param name="context"></param>
    /// 

    public CountDownTimer countDownTimer = new(300);
    public void Enter(TeamScoreGameEnd context)
    {
        // The ending scores pop up: players can't move - 3 seconds
        // Implement logic to display scores

        countDownTimer.StartTimer();
    }

    public void Update(TeamScoreGameEnd context)
    {
        // Transition to WaitForInputState after some condition, e.g., a timer
        context.StateMachine.ChangeState(new WaitForInputState());
    }

    public void FixedUpdate(TeamScoreGameEnd context)
    {
        // Handle FixedUpdate logic if needed
        countDownTimer.CountDown();
        if (countDownTimer.IsFinished())
        {
            context.StateMachine.ChangeState(new WaitForInputState());
        }
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for ShowScores state
    }
}
