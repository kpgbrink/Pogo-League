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
            playerSparData.Players.ForEach(player =>
            {
                player.PlayerControlledObjects.ForEach(obj =>
                {
                    //obj.
                    var freezableComponents = obj.GetComponents<Freezable>();
                    foreach (var freezable in freezableComponents)
                    {
                        freezable.Freeze();
                    }
                });
            });
        }
    }
}
