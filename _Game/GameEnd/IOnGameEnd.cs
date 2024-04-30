using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnGameEnd
{
    bool GameEnded { get; set; }

    void OnGameEnd(GameEndStates gameEndState, List<PlayerSpar> playerSparsWinner);
}

