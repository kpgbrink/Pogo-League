using UnityEngine;
using System.Collections;
using AutoLevelMenu.Events;
using Assets.Scripts._Game.GameStart;

/// <summary>
/// This kinda just makes timers run when they can't listen to things because they are inactived.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private GameStartCountDown gameStartCountDown;

    [SerializeField]
    private StartWaitingForCountDown startWaitingForCountDown;

    [SerializeField]
    private float goalPauseDuration = 2f; // Duration to pause after a goal before restarting the countdown

    [SerializeField]
    GameEvent startWaitingForCountdownEvent;

    [SerializeField]
    GameEvent gamePauseEvent;

    public void OnGoal()
    {
        // Handle the stuff once the goal happens.
        StartWaitingForCountdown();
    }

    public void StartWaitingForCountdown()
    {
        startWaitingForCountdownEvent.Raise();
        // Another game object with a timer will have the timer to start waiting for countdown
        startWaitingForCountDown.StartCountdown();
    }

    public void StartCountDown()
    {
        gamePauseEvent.Raise();
        gameStartCountDown.StartCountdown();
    }

    public void StartPostGameWait()
    {
    }
}
