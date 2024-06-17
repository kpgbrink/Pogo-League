using AutoLevelMenu;
using AutoLevelMenu.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixedUpdateClock : MonoBehaviour
{
    [SerializeField]
    TimeText[] timeTexts;

    // Start is called before the first frame update
    public bool GameStarted { get; private set; } = false;

    public bool GamePaused { get; private set; } = false;

    public bool GameEnded { get; private set; } = false;

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

    public void SetGameGoing(bool going)
    {
        GameStarted = going;
        if (going)
        {
            GamePaused = false;
            ResetValuesOnSceneInit.GameBeforeStart = false;
        }
    }

    public void SetGamePause(bool pause)
    {
        Debug.Log($"SetGamePause: {pause}");
        GamePaused = pause;
    }

    public void SetGameEnd(bool end)
    {
        GameEnded = end;
        if (end)
        {
            ResetValuesOnSceneInit.GameAfterEnd = true;
        }
    }

    void FixedUpdate()
    {
        if (GameStarted && !GamePaused)
        {
            Clock++;
        }
    }

    private void Update()
    {
        foreach(var timeText in timeTexts)
        {
            timeText.SetText(Clock);
        }
    }
}
