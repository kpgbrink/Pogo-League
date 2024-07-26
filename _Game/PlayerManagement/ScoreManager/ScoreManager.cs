using AutoLevelMenu.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Handle checking if game is over and scoring the game at the end.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // If zero don't use it
    [SerializeField]
    protected int maxPoints = 0;

    [SerializeField]
    protected GameEvent gameEndEvent;

    [SerializeField]
    ScoreText[] scoreTexts;

    protected Scores Scores { get; set; } = new Scores();

    [SerializeField]
    protected GameEnd gameEnd;

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

    public virtual GameEndStates? OnCheckGameEnd()
    {
        throw new NotImplementedException();
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
