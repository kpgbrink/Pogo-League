using AutoLevelMenu.Events;
using UnityEngine;

public class FixedUpdateClock : MonoBehaviour
{
    [SerializeField]
    TimeText[] timeTexts;

    [SerializeField]
    TimeText[] timerTexts;

    public bool IsOvertime { get; private set; } = false;
    public bool GameCountdownGoing { get; private set; } = false;
    public bool GamePaused { get; private set; } = false;
    public bool GameWaitingForBallHit { get; private set; } = false;
    public int Clock { get; private set; }

    // If zero don't use it
    [SerializeField]
    public int fixedUpdateClockMax = 0;

    [SerializeField]
    GameEvent startWaitingForBallToTouchGroundToEndGame;

    [SerializeField]
    GameEvent startOvertime;

    [SerializeField]
    ScoreManager playerScoreManager;

    [SerializeField]
    GameEvent gameEndEvent;

    public int FixedUpdateCountdown
    {
        get
        {
            return fixedUpdateClockMax - Clock;
        }
    }

    bool AlreadySetGameCountDownEnd { get; set; } = false;


    public class ResetableValuesOnSceneInit
    {
        public bool GameBeforeStart { get; set; } = true;
        public bool GameAfterEnd { get; set; } = false;
    }

    public ResetableValuesOnSceneInit ResetValuesOnSceneInit { get; set; }

    public void Start()
    {
        ResetValuesOnSceneInit = new ResetableValuesOnSceneInit();
    }

    public void OnSetGameStarted()
    {
        GamePaused = false;
        ResetValuesOnSceneInit.GameBeforeStart = false;
    }

    public void OnStopGameCountdown()
    {
        GameCountdownGoing = false;
    }

    public void OnStartGameCountdown()
    {
        GameCountdownGoing = true;
    }

    public void OnSetGamePause(bool pause)
    {
        GamePaused = pause;
        GameCountdownGoing = !pause;
    }

    public void OnSetGameWaitingForBallHit(bool waiting)
    {
        GameWaitingForBallHit = waiting;
    }

    void FixedUpdate()
    {
        if (!ResetValuesOnSceneInit.GameAfterEnd)
        {
            ClockCounting();
        }
    }

    void ClockCounting()
    {
        if (GameCountdownGoing && !GamePaused)
        {
            Clock++;
        }
        // check if the fixedupdate clock is set to zero
        if (fixedUpdateClockMax != 0 && Clock >= fixedUpdateClockMax)
        {
            // Set the clock back 1 to keep it off of zero until the ball touches the ground
            if (!AlreadySetGameCountDownEnd)
            {
                GameCountdownGoing = false;
                Clock = fixedUpdateClockMax - 1;
                startWaitingForBallToTouchGroundToEndGame.Raise();
                AlreadySetGameCountDownEnd = true;
            }
        }
    }

    public void OnBallTouchedGroundOrScoredToEndGame()
    {
        Clock = fixedUpdateClockMax;
        MaybeSetGameEnd();
    }

    public void OnBallScoredInOvertimeToEndGame()
    {
        MaybeSetGameEnd();
    }

    void MaybeSetGameEnd()
    {
        // Run check if the game should end
        var gameEndState = playerScoreManager.OnCheckGameEnd();
        // If the game should not end then we need to start the overtime. where it keeps going until the ball is scored.
        if (gameEndState.HasValue)
        {
            ResetValuesOnSceneInit.GameAfterEnd = true;
        }
        // If the game should end then we need to end the game.
        else
        {
            StartOvertime();
        }
    }

    void StartOvertime()
    {
        // Raise overtime event
        startOvertime.Raise();
        IsOvertime = true;
        GameCountdownGoing = true;
    }

    void Update()
    {
        UpdateTexts();
    }

    void UpdateTexts()
    {
        UpdateTimerTexts();
        UpdateTimeTexts();
    }

    void UpdateTimerTexts()
    {
        foreach (var timer in timerTexts)
        {
            timer.SetText(FixedUpdateCountdown);
        }
    }

    void UpdateTimeTexts()
    {
        foreach (var timeText in timeTexts)
        {
            timeText.SetText(Clock);
        }
    }
}
