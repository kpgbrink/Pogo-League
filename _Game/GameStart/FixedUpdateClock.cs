using AutoLevelMenu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixedUpdateClock : MonoBehaviour
{
    [SerializeField]
    TimeText[] timeTexts;

    // Start is called before the first frame update
    public bool GameGoing { get; private set; } = false;

    public int Clock { get; private set; }

    private void Start()
    {
        Time.timeScale = Global.TimeScaleBeforeGameStart;
    }

    public void SetGameGoing()
    {
        Time.timeScale = 1;
        GameGoing = true;
    }

    public void SetGameGoingFalse()
    {
        GameGoing = false;
    }

    void FixedUpdate()
    {
        if (GameGoing)
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
