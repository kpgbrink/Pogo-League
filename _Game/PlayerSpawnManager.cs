using Assets.Scripts;
using Assets.Scripts.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField]
    int lives = 5;
    public int Lives
    {
        get => lives;
        set => lives = value;
    }

    [SerializeField]
    bool infiniteLives = false;

    public bool InfiniteLives
    {
        get => infiniteLives;
        set => infiniteLives = value;
    }

    [SerializeField]
    int respawnFixedFramesStart = 100;
    public int RespawnFixedFramesStart
    {
        get => respawnFixedFramesStart;
        set => respawnFixedFramesStart = value;
    }

    public enum SpawnTypes
    {
        RoundRobin,
        FurthestFromAll,
        FurthestFromEnemy,
    }
    public SpawnTypes firstSpawnType;
    public SpawnTypes spawnType;
    public PlayerSparData playerSparManager;

    List<int> PlayerSpawnIndexes { get; set; } = new List<int>();

    List<List<Transform>> ListSpawns
    {
        get
        {
            var playerListSpawns = new List<List<Transform>>();
            foreach (Transform playerSpawnTeam in transform)
            {
                var playerSpawns = new List<Transform>();
                foreach (Transform spawnPoint in playerSpawnTeam)
                {
                    playerSpawns.Add(spawnPoint);
                }
                playerListSpawns.Add(playerSpawns);
            }
            // Add list of zero's if values do not exist at the end. Keep extending values because that doesn't matter
            PlayerSpawnIndexes.AddRange(Enumerable.Repeat(0, Math.Max(0, playerListSpawns.Count - PlayerSpawnIndexes.Count)));
            //Debug.Log(PlayerSpawnIndexes);
            return playerListSpawns;
        }
    }

    public Transform GetSpawnPoint(PlayerSpar playerSpar, int playerNumber, bool firstSpawn = false)
    {
        // Set to chosen team before
        playerSpar.Team = playerSpar.ChoosenTeam;
        FixTeams();
        var usedSpawnType = firstSpawn ? firstSpawnType : spawnType;
        Debug.Log($"Spawn type {usedSpawnType}");
        switch (usedSpawnType)
        {
            case SpawnTypes.RoundRobin:
                // Make round robin work first because why not
                // First I must get the index of the place I will spawn the people at
                // If there is only a list of one list then it is 0
                return FuncUtil.Invoke(() =>
                {
                    var spawnListIndex = 0;
                    // Player teams are forced
                    if (ListSpawns.Count > 1)
                    {
                        spawnListIndex = playerSpar.Team - 1;
                    }
                    // If over go back to zero
                    if (PlayerSpawnIndexes[spawnListIndex] >= ListSpawns[spawnListIndex].Count)
                    {
                        PlayerSpawnIndexes[spawnListIndex] = 0;
                    }
                    var spawnIndex = ListSpawns[spawnListIndex][PlayerSpawnIndexes[spawnListIndex]];
                    PlayerSpawnIndexes[spawnListIndex]++;
                    Debug.Log($"Spawn index {spawnIndex.position.x} {spawnIndex.position.y}");
                    return spawnIndex;
                });
            case SpawnTypes.FurthestFromAll:

                break;
            case SpawnTypes.FurthestFromEnemy:

                break;
        }
        throw new Exception("Should have returned a respawn type");
    }

    void FixTeams()
    {
        if (ListSpawns.Count == 0)
        {
            Debug.LogError("There are no spawn places? Fix this level.");
            return;
        }
        // If player teams are forced
        else if (ListSpawns.Count > 1)
        {
            // If player Team number is outside fix it
            playerSparManager.PlayerSpars.ForEach(o =>
            {
                if (ListSpawns.Count < o.Team)
                {
                    o.Team = 0;
                }
            });

            // Assign players without team randomly to randomly lowest team
            while (true)
            {
                var sparNotAssignedToScenes = playerSparManager.PlayerSpars.Where(o => o.Team == 0).ToList();
                if (sparNotAssignedToScenes.Count == 0) break;
                // Randomly assign all known players to team.
                sparNotAssignedToScenes.ChooseRandom().Team = FuncUtil.Invoke(() =>
                {
                    // Get lowest team. Randomly assign to lowest.
                    // Fix this
                    var teamCounts1 = playerSparManager.TeamCounts(ListSpawns.Count);
                    var minTeamCount = teamCounts1.Min(t => t.Count);
                    var minTeams = teamCounts1.Where(t => t.Count == minTeamCount).ToList();
                    return minTeams.ChooseRandom().Team;
                });
            }

            // Fix teams if there are not enough spawn points for a team
            // Try moving to another team. If fails then fail it.
            var teamCounts = playerSparManager.TeamCounts(ListSpawns.Count);
            bool PlayerTeamTooHigh(int index) => teamCounts[index].Count > ListSpawns[index].Count;
            for (var i = 0; i < teamCounts.Count; i++)
            {
                // Team count too high! move to another team
                if (PlayerTeamTooHigh(i))
                {
                    for (var e = 0; e < teamCounts.Count; e++)
                    {
                        // I can move you to this team
                        while (!PlayerTeamTooHigh(e))
                        {
                            // Team that you must be moved to
                            var moveFromTeam = teamCounts[i].Team;
                            var moveToTeam = teamCounts[e].Team;
                            // Chose random person from the team and move it to the move team
                            var moveFromTeamPlayers = playerSparManager.PlayerSpars.Where(ps => ps.Team == moveFromTeam).ToList();
                            moveFromTeamPlayers.ChooseRandom().Team = moveToTeam;
                        }
                    }
                }
                if (PlayerTeamTooHigh(i))
                {
                    Debug.LogError("Well I failed to move the people to give them enough spawns. There are not enough spawns I guess.");
                }
            }
        }
        else if (ListSpawns.Count == 1)
        {
            // Check if there is enough spawn points for the number of players
            if (ListSpawns[0].Count < playerSparManager.PlayerSpars.Count)
            {
                Debug.LogError("Not enough spawn points for the number of players");
            }
        }
        // Check if everyone is only on one team and more than one person. If you can try to assign a random person to a different team.
        if (playerSparManager.PlayerSpars.Count != 1)
        {
            var teamCounts = playerSparManager.TeamCounts(ListSpawns.Count);
            var teamsWithCounts = teamCounts.Where(t => t.Count > 0).ToList();
            var noTeamCount = playerSparManager.PlayerSpars.Where(p => p.Team == 0).ToList().Count;
            if (teamsWithCounts.Count == 1 && noTeamCount == 0)
            {
                playerSparManager.PlayerSpars.ChooseRandom().Team = 0;
                if (ListSpawns.Count > 1)
                {
                    // Set random player to zero then rerun this method.
                    FixTeams();
                } else if(ListSpawns.Count == 1)
                {
                    // Already set one random person to team 0.
                }
            }
        }
    }
}
