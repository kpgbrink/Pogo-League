using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamScoreManager : ScoreManager
{
    public override GameEndStates? OnCheckGameEnd()
    {
        // Check if game is in win state
        var (gameEndState, playerSpars) = FuncUtil.Invoke(() =>
        {
            var checkplayerScoreEnd = CheckGameTeamScoreEnd();
            if (checkplayerScoreEnd.gameEndState != null) return checkplayerScoreEnd;
            return (null, null);
        });
        Debug.Log($"Game end state: {gameEndState}");
        if (gameEndState != null)
        {
            // Run the end game stuff
            // For end game I will have it take all the winning players
            // Do something special to them on win
            // Everyone will be respawned but maybe into a special thing?
            // I will have an on win thing for each people
            // IOnWin
            // IOnLose
            gameEnd.OnGameEnd(
                (GameEndStates)gameEndState,
                playerSpars);
            gameEndEvent.Raise();
        }
        return gameEndState;
    }

    /// <summary>
    /// Check if the game ends from the score going up.
    /// </summary>
    /// <returns></returns>
    (GameEndStates? gameEndState, List<PlayerSpar> playerSpars) CheckGameTeamScoreEnd()
    {
        // Get the highest team scores
        var topTeamScores = Scores.GetHighestTeamAndScoreList();

        // If there are no team scores, return null
        if (topTeamScores.Count == 0)
        {
            return (null, null);
        }

        // Get the top team score and the list of top teams
        var highestScore = topTeamScores[0].score;
        var teamsWithHighestScore = topTeamScores.Where(p => p.score == highestScore).Select(p => p.teamNumber).ToList();

        // Check if the highest score is unique
        if (teamsWithHighestScore.Count == 1)
        {
            var topTeamNumber = teamsWithHighestScore[0];

            // Get the PlayerSpars for the top team
            var topTeamPlayerSpars = playerSparData.PlayerSpars.Where(ps => ps.Team == topTeamNumber).ToList();

            // Return the game end state and the player spars
            return (GameEndStates.TeamScoreEnd, topTeamPlayerSpars);
        }

        // If no condition is met for ending the game, return null
        return (null, null);
    }


}
