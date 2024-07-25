using Assets.Scripts;
using AutoLevelMenu.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Handle checking if game is over and scoring the game at the end.
/// </summary>
public class PlayerScoreManager : MonoBehaviour
{
    // If zero don't use it
    [SerializeField]
    int maxPoints = 0;

    [SerializeField]
    bool overtime = true;

    [SerializeField]
    GameEvent gameEndEvent;

    [SerializeField]
    ScoreText[] scoreTexts;

    Scores Scores { get; set; } = new Scores();

    bool Overtime { get; set; } = false;

    IOnGameEnd GameEnd => GameObject.Find("/GameEnd").GetComponent<IOnGameEnd>();

    public PlayerSparData playerSparData;

    public void AddScore(int teamNumber, int playerNumber, int addAmount = 1)
    {
        Debug.Log($"add score\n team number: {teamNumber} player number: {playerNumber}");
        Scores.AddScore(teamNumber, playerNumber, addAmount);
        UpdateScoreTexts();
    }

    public void UpdateScoreTexts()
    {
        var teamScores = Scores.TeamScores;
        var playerScores = Scores.PlayerScores;
        var teamScoreTexts = scoreTexts.Where(st => st.teamNumber != 0);
        SetScoreTexts(teamScoreTexts, teamScores, (ScoreText scoreText) => scoreText.teamNumber);
        var playerScoreTexts = scoreTexts.Where(st => st.teamNumber == 0);
        SetScoreTexts(playerScoreTexts, playerScores, (ScoreText ScoreText) => ScoreText.playerNumber);
    }

    void SetScoreTexts(IEnumerable<ScoreText> scoreTexts, Dictionary<int, int> scores, Func<ScoreText, int> getKey)
    {
        foreach (var scoreText in scoreTexts)
        {
            // Try to find the score
            if (scores.TryGetValue(getKey(scoreText), out var teamScoreAmount))
            {
                scoreText.SetText($"{teamScoreAmount}");
            }
        }
    }

    public GameEndStates? OnCheckGameEnd()
    {
        // Check if game is in win state
        var (gameEndState, playerSpars) = FuncUtil.Invoke(() =>
        {
            var checkPlayerDeathEnd = CheckGamePlayerDeathEnd();
            if (checkPlayerDeathEnd.gameEndState != null) return checkPlayerDeathEnd;
            Debug.Log("Not player death");
            var checkplayerScoreEnd = CheckGamePlayerScoreEnd();
            if (checkplayerScoreEnd.gameEndState != null) return checkplayerScoreEnd;
            return (null, null);
        });
        if (gameEndState != null)
        {
            // Run the end game stuff
            // For end game I will have it take all the winning players
            // Do something special to them on win
            // Everyone will be respawned but maybe into a special thing?
            // I will have an on win thing for each people
            // IOnWin
            // IOnLose
            GameEnd.OnGameEnd(
                (GameEndStates)gameEndState,
                playerSpars);
            gameEndEvent.Raise();
        }
        return gameEndState;
    }

    /// <summary>
    /// Checks if the game ends from a player death.
    /// </summary>
    /// <returns></returns>
    (GameEndStates? gameEndState, List<PlayerSpar> playerSpars) CheckGamePlayerDeathEnd()
    {
        // Get all that are still alive. Check if they are all on the same team 
        var notCompletelyDeadPlayers = playerSparData.PlayerSpars.Where(p => p.GetComponent<Player>().ResetValuesOnSceneInit.CompletelyDead == false).ToList();
        //Debug.Log(notCompletelyDeadPlayers.Count);
        // What if everyone is dead. No one wins but game is over.
        if (notCompletelyDeadPlayers.Count == 0)
        {
            // End state is true
            return (GameEndStates.EveryoneDead, null);
        }
        // Check if everyone who is alive is on the same team and team not 0. That team wins
        var playerTeamFirst = notCompletelyDeadPlayers[0].Team;
        if (playerTeamFirst != 0 &&
            notCompletelyDeadPlayers.Where(p => p.Team == playerTeamFirst).ToList().Count == notCompletelyDeadPlayers.Count)
        {
            // A team has one.
            return (
                GameEndStates.TeamWins,
                playerSparData.PlayerSpars.Where(p => p.Team == playerTeamFirst).ToList());
        }
        // Check if only one person is alive. That person wins.
        if (notCompletelyDeadPlayers.Count == 1)
        {
            // A lone person has one
            return (
                GameEndStates.PlayerWins,
                notCompletelyDeadPlayers
                );
        }
        return (null, null);
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

public enum GameEndStates
{
    TeamScoreEnd,
    EveryoneDead,
    TeamWins,
    PlayerWins,
}

public enum GameEndForceTypes
{
    PlayerScoreEnd,
    PlayerDeathEnd
}
