using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts._Game.PlayerManagement
{
    public class TeamPlayerManager : MonoBehaviour
    {
        [SerializeField]
        PlayerSparData playerSparData;
        // Freeze all players
        public void FreezeAllPlayers()
        {
            ToggleFreezePlayers(true);
        }

        public void UnFreezeSomePlayers(List<PlayerSpar> somePlayers)
        {
            ToggleFreezePlayers(false, somePlayers);
        }


        void ToggleFreezePlayers(bool freezeOn, List<PlayerSpar> players = null)
        {
            List<global::Player> test = null;
            if (players is not null)
            {
                // Filter out the players that have the specific spart data
                test = playerSparData.Players.Where(p => players.Any(p2 => p.playerSpar == p2)).ToList();
            }
            else
            {
                test = playerSparData.Players;
            }
            test.ForEach(player =>
            {
                player.PlayerControlledObjects.ForEach(obj =>
                {
                    //obj.
                    var freezableComponents = obj.GetComponents<Freezable>();
                    foreach (var freezable in freezableComponents)
                    {
                        if (freezeOn)
                        {
                            freezable.Freeze();
                        }
                        else
                        {
                            freezable.Unfreeze();
                        }
                    }
                });
            });
        }
    }
}
