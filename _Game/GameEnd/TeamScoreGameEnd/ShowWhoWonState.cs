using UnityEngine;
public class ShowWhoOneState : IState<TeamScoreGameEnd>
{
    public CountDownTimer countDownTimer = new(300);
    public void Enter(TeamScoreGameEnd context)
    {
        // Blue/Red team wins: The team that wins can move for 3 seconds - 5 seconds
        // Implement logic to allow winning team movement

        countDownTimer.StartTimer();

        Debug.Log("Show who won");
        // Show who won text 
        context.winnerText.SetActive(true);
        // If winningTeam is 1 , then blue team won, else red team won
        context.winnerTextMeshPro.text = context.WinningTeam == 1 ? "Blue Team Wins!" : "Red Team Wins!";
        // Set text color to color of winner
        context.winnerTextMeshPro.color = context.WinningTeam == 1 ? Color.blue : Color.red;
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
            context.StateMachine.ChangeState(new ShowScoresState());
        }
    }

    public void Exit(TeamScoreGameEnd context)
    {
        // Cleanup logic for AllowWinningTeamMovement state
        context.winnerText.SetActive(false);
    }
}