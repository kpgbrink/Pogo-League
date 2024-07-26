using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerScoreManager : ScoreManager
{
    public override GameEndStates? OnCheckGameEnd()
    {
        // Check if game is in win state
        var (gameEndState, playerSpars) = FuncUtil.Invoke(() =>
        {
            var checkplayerScoreEnd = CheckGamePlayerScoreEnd();
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
    (GameEndStates? gameEndState, List<PlayerSpar> playerSpars) CheckGamePlayerScoreEnd()
    {
        // Go through the list of things.
        var highestScore = Scores.GetHighestScore();
        if ((highestScore >= maxPoints && maxPoints != 0))
        {
            var topPlayerScores = Scores.GetHighestPlayerKeyAndScoreList();
            var topTeamScores = Scores.GetHighestTeamAndScoreList();
            List<PlayerSpar> TopPlayerScoresSpars()
            {
                var topPlayers = topPlayerScores.Select(p => p.playerNumber).ToList();
                return playerSparData.Players.Where(p => topPlayers.Contains(p.PlayerNumber)).Select(p => p.playerSpar).ToList();
            }
            List<PlayerSpar> TopTeamPlayerScoresSpars()
            {
                var topTeamPlayers = topTeamScores.Select(p => p.teamNumber).ToList();
                return playerSparData.PlayerSpars.Where(ps => topTeamPlayers.Contains(ps.Team)).ToList();
            }
            (GameEndStates? gameEndState, List<PlayerSpar> playerSpars) ReturnPlayers(List<PlayerSpar> playerSpars)
            {
                return (GameEndStates.TeamScoreEnd, playerSpars);
            }
            if (topPlayerScores.Count == 0 && topTeamScores.Count == 0)
            {
                return (GameEndStates.TeamScoreEnd, null);
            }
            if (topPlayerScores.Count > 0 && topTeamScores.Count > 0)
            {
                var topPlayerScore = topPlayerScores[0].score;
                var topTeamScore = topTeamScores[0].score;
                if (topPlayerScore > topTeamScore)
                {
                    return ReturnPlayers(TopPlayerScoresSpars());
                }
                else if (topTeamScore > topPlayerScore)
                {
                    return ReturnPlayers(TopTeamPlayerScoresSpars());
                }
                else if (topPlayerScore == topTeamScore)
                {
                    var topPlayerScoresSpars = TopPlayerScoresSpars();
                    topPlayerScoresSpars.AddRange(TopTeamPlayerScoresSpars());
                    return ReturnPlayers(topPlayerScoresSpars);
                }
            }
            else if (topPlayerScores.Count > 0)
            {
                return ReturnPlayers(TopPlayerScoresSpars());
            }
            else if (topTeamScores.Count > 0)
            {
                return ReturnPlayers(TopTeamPlayerScoresSpars());
            }
        }
        return (null, null);
    }
}
