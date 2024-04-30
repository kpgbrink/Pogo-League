using AutoLevelMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = nameof(PlayerSparData), menuName = Global.AssetMenuPathStart.gameData + "/" + nameof(PlayerSparData))]
public class PlayerSparData : ScriptableObject
{
    public List<PlayerSpar> PlayerSpars { get; set; } = new List<PlayerSpar>();

    public List<Player> Players => PlayerSpars.Select(p => p.transform.GetComponent<Player>()).ToList();

    public int TeamCount(int team)
    {
        return PlayerSpars.Where(p => p.Team == team).Count();
    }

    public List<(int Team, int Count)> TeamCounts(int possibleTeamCounts)
    {
        var teams = Enumerable.Repeat(1, possibleTeamCounts).Select((_, i) => i + 1);
        return teams.Select(t => (t, TeamCount(t))).ToList();
    }

    /// <summary>
    /// Each team not including 0. Which is not considered a team.
    /// </summary>
    /// <returns></returns>
    public List<int> AllTeams()
    {
        return PlayerSpars.Select(p => p.Team).Where(t => t != 0).OrderBy(o => o).Distinct().ToList();
    }

    public List<int> SinglePlayers()
    {
        return Players.Where(p => p.playerSpar.Team == 0).Select(p => p.PlayerNumber).ToList();
    }

    private void OnDisable()
    {
        PlayerSpars = new List<PlayerSpar>();
    }
}