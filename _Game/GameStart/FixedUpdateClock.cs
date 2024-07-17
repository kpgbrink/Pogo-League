using UnityEngine;

public class FixedUpdateClock : MonoBehaviour
{
    [SerializeField]
    TimeText[] timeTexts;

    public bool GameCountdownGoing { get; private set; } = false;
    public bool GamePaused { get; private set; } = false;
    public bool GameEnded { get; private set; } = false;
    public bool GameWaitingForBallHit { get; private set; } = false;
    public int Clock { get; private set; }

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
        Debug.Log("SetGameWaitingForBallHit: " + waiting);
        GameWaitingForBallHit = waiting;
    }

    void FixedUpdate()
    {
        if (GameCountdownGoing && !GamePaused)
        {
            Clock++;
        }
    }

    void Update()
    {
        foreach (var timeText in timeTexts)
        {
            timeText.SetText(Clock);
        }
    }
}
