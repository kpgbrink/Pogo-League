using System.Collections.Generic;
using UnityEngine;

public class SimpleGameEnd : MonoBehaviour, IOnGameEnd
{
    [SerializeField]
    CountDownTimer ToDataScreenTimer;

    public PlayerSparData playerSparData;

    public GameObject winnerText;

    public GameObject gameDataEnd;

    public bool GameEnded { get; set; }

    public void Start()
    {
        ToDataScreenTimer.StopTimer();
    }

    public void OnGameEnd(GameEndStates gameEndState, List<PlayerSpar> playerSparsWinner)
    {
        GameEnded = true;
        winnerText.SetActive(true);
        playerSparData.Players.ForEach(player =>
        {
            player.DestroyAllPlayerControlledObjects();
        });
        // Show the winning players be respawned
        playerSparsWinner?.ForEach(playerSpar =>
        {
            var player = playerSpar.GetComponent<Player>();
            player.Respawn();
        });
        ToDataScreenTimer.StartTimer();
    }

    void ShowDataScreen()
    {
        gameDataEnd.SetActive(true);
    }

    void FixedUpdate()
    {
        ToDataScreenTimer.CountDown();
        if (ToDataScreenTimer.IsFinished())
        {
            ToDataScreenTimer.StopTimer();
            winnerText.SetActive(false);
            ShowDataScreen();
        }
    }
}
