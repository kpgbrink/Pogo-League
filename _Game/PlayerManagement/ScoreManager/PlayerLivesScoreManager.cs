using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLivesScoreManager : ScoreManager
{
    public override GameEndStates? OnCheckGameEnd()
    {
        // Check if game is in win state
        var (gameEndState, playerSpars) = FuncUtil.Invoke(() =>
        {
            var checkPlayerDeathEnd = CheckGamePlayerDeathEnd();
            if (checkPlayerDeathEnd.gameEndState != null) return checkPlayerDeathEnd;
            Debug.Log("Not player death");
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
}
