using Assets.Scripts._Game.PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public class TeamScoreGameEnd : GameEnd
{
    [SerializeField]
    CountDownTimer ToDataScreenTimer;
    public PlayerSparData playerSparData;
    public TeamPlayerManager teamPlayerManager;
    public GameObject winnerText;
    public GameObject gameOverText;
    public GameObject gameDataEnd;

    public StateMachine<TeamScoreGameEnd> StateMachine { get; private set; }

    public void Start()
    {
        ToDataScreenTimer.StopTimer();
        StateMachine = new StateMachine<TeamScoreGameEnd>(this);
        StateMachine.ChangeState(new EmptyState()); // Start with an empty state
    }

    public override void OnGameEnd(GameEndStates gameEndState, List<PlayerSpar> playerSparsWinner)
    {
        // Freeze all of the rigidbodies for a moment.
        // Show GAME OVER -- 3 seconds
        // Blue/Red team wins: The team that wins can move for 3 seconds - 5 seconds
        // The ending scores pop up: players can't move - 3 seconds
        // Press A to continue: now you can leave the game
        StateMachine.ChangeState(new FreezePlayersState());
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
}
