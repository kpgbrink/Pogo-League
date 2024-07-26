using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public virtual void OnGameEnd(GameEndStates gameEndState, List<PlayerSpar> playerSparsWinner)
    {
        Debug.Log("Game End On Game End");
    }
}
