using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalScoreDeviceStatistics : MonoBehaviour
{

    ResetableValuesOnRespawn resetValuesOnGoal = new();

    public void RecordPlayerHit(PlayerObject playerObject, int clock)
    {
        resetValuesOnGoal.PlayerHitDatas.Add(new PlayerHitData()
        {
            TimeTouched = clock,
            Player = playerObject.Player,
            PlayerSpar = playerObject.PlayerSpar,
        });
    }

    public PlayerHitData GetLastHitPlayer(int? goalTeam, int? playerNumber)
    {
        return resetValuesOnGoal.GetLastHitPlayerData(goalTeam, playerNumber);
    }

    class ResetableValuesOnRespawn
    {

        public List<PlayerHitData> PlayerHitDatas = new();
        public PlayerHitData GetLastHitPlayerData(int? goalTeam, int? playerNumber)
        {
            if (goalTeam != null)
            {
                return PlayerHitDatas.Where(ph => ph.PlayerSpar.Team != goalTeam).LastOrDefault();
            }
            if (playerNumber != null)
            {
                return PlayerHitDatas.Where(ph => ph.Player.PlayerNumber != playerNumber).LastOrDefault();
            }
            return PlayerHitDatas.LastOrDefault();
        }
    }

    public class PlayerHitData
    {
        public int TimeTouched { get; set; }
        public Player Player { get; set; }
        public PlayerSpar PlayerSpar { get; set; }
    }
}
