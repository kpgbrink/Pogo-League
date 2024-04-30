using AutoLevelMenu;
using AutoLevelMenu.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountDown : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    int StartTimer = 3;

    [SerializeField]
    string EndingString = "Start";

    [SerializeField]
    float countDownMultiplier = 1f;

    float CountDown;

    bool gameStartRaised = false;

    [SerializeField]
    GameEvent gameStartEvent;

    // Start is called before the first frame update
    void Start()
    {
        CountDown = StartTimer + 1;
    }

    void Update()
    {
        if (CountDown < 0)
        {
            gameObject.SetActive(false); 
        }
        CountDown -= Time.deltaTime / Global.TimeScaleBeforeGameStart * countDownMultiplier;

        var countDown = Math.Min(StartTimer, (int) CountDown);
        text.text = countDown != 0 ? $"{countDown}" : EndingString;
        if (!gameStartRaised && countDown <= 0)
        {
            gameStartEvent.Raise();
            gameStartRaised = true;
        }
    }
}
