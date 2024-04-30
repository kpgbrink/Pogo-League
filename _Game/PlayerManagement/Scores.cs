using Assets.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Scores
{
    public Dictionary<int /*team number*/, int /*score*/> PlayerScores { get; private set; } = new Dictionary<int, int>();

    public Dictionary<int /*player number*/, int/*score*/> TeamScores { get; private set; } = new Dictionary<int, int>();

    public void AddScore(int teamNumber, int playerNumber, int addAmount = 1)
    {
        if (teamNumber == 0)
        {
            AddScoreToDictionary(PlayerScores, playerNumber, addAmount);
            return;
        }
        AddScoreToDictionary(TeamScores, teamNumber, addAmount);
    }

    void AddScoreToDictionary(Dictionary<int, int> dictionary, int key, int addAmount)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            dictionary[key] = value + addAmount;
            return;
        }
        dictionary[key] = addAmount;
    }

    public int GetHighestScore()
    {
        return Math.Max(PlayerScores.Values.ToList().MaxIntOrZero(), TeamScores.Values.ToList().MaxIntOrZero());
    }

    public List<(int? playerNumber, int? score)> GetHighestPlayerKeyAndScoreList()
    {
        return GetHighestScoreList(PlayerScores);
    }

    public List<(int? teamNumber, int? score)> GetHighestTeamAndScoreList()
    {
        return GetHighestScoreList(TeamScores);
    }

    List<(int? teamNumber, int? score)> GetHighestScoreList(Dictionary<int, int> dict)
    {
        var value = dict.MaxValueIntKey();
        if (value == null) return new List<(int? teamNumber, int? score)>();
        return dict.Where(p => p.Value == value).Select(o => (playerNumber: (int?)o.Key, score: (int?)o.Value)).ToList();
    }
}
