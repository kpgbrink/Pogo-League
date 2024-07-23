using AutoLevelMenu.Events;
using UnityEngine;

public class FixedUpdateClock : MonoBehaviour
{
    [SerializeField]
    TimeText[] timeTexts;

    [SerializeField]
    TimeText[] timerTexts;

    public bool GameCountdownGoing { get; private set; } = false;
    public bool GamePaused { get; private set; } = false;
    public bool GameEnded { get; private set; } = false;
    public bool GameWaitingForBallHit { get; private set; } = false;
    public int Clock { get; private set; }

    // If zero don't use it
    [SerializeField]
    public int fixedUpdateClockMax = 0;

    [SerializeField]
    GameEvent startWaitingForBallToTouchGroundToEndGame;


    [SerializeField]
    PlayerScoreManager playerScoreManager;

    int FixedUpdateCountdown { get; set; } = 0;

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

    void CountDownTimer()
    {
        FixedUpdateCountdown = fixedUpdateClockMax - Clock;
    }

    public void SetGameStarted()
    {
        GamePaused = false;
        ResetValuesOnSceneInit.GameBeforeStart = false;
    }

    public void StopGameCountdown()
    {
        GameCountdownGoing = false;
    }

    public void StartGameCountdown()
    {
        GameCountdownGoing = true;
    }

    public void SetGamePause(bool pause)
    {
        GamePaused = pause;
        GameCountdownGoing = !pause;
    }

    public void SetGameEnd(bool end)
    {
        GameEnded = end;
        if (end)
        {
            ResetValuesOnSceneInit.GameAfterEnd = true;
        }
    }

    public void SetGameWaitingForBallHit(bool waiting)
    {
        GameWaitingForBallHit = waiting;
    }

    void FixedUpdate()
    {
        CountDownTimer();
        if (GameCountdownGoing && !GamePaused)
        {
            Clock++;
        }
        // check if the fixedupdate clock is set to zero
        if (fixedUpdateClockMax != 0 && Clock >= fixedUpdateClockMax)
        {
            GameCountdownGoing = false;
            // Set the clock back 1 to keep it off of zero until the ball touches the ground
            Clock = fixedUpdateClockMax - 1;
            if (!AlreadySetGameCountDownEnd)
            {
                startWaitingForBallToTouchGroundToEndGame.Raise();
                AlreadySetGameCountDownEnd = true;
            }
        }
    }

    public void OnBallTouchedGroundToEndGame()
    {
        Clock = fixedUpdateClockMax;
        // RUn check if the game should end
        playerScoreManager.OnCheckGameEnd();
        // If the game should not end then we need to start the overtime. where it keeps going until the ball is scored
    }

    void Update()
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
