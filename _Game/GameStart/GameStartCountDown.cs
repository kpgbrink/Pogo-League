﻿using AutoLevelMenu.Events;
using TMPro;
using UnityEngine;

public class GameStartCountDown : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    float countDownMultiplier = 1f; // THis is kinda for testing purposes. I don't want to wait at all when I test.

    //bool gameStartRaised = false;

    [SerializeField]
    GameEvent gameStartEvent;

    // This will be the four seconds for the thing
    CountDownTimer countDownTimer = new(300);

    CountDownTimer goTextLingerTime = new(100);

    private void Start()
    {
        countDownTimer.StopTimer();
        goTextLingerTime.StopTimer();
    }

    private void FixedUpdate()
    {
        CountDownTimer();
        GoTextLingerTimer();
    }

    private void GoTextLingerTimer()
    {
        if (!goTextLingerTime.GetIsGoing()) return;
        goTextLingerTime.CountDown();
        if (goTextLingerTime.IsFinished())
        {
            gameObject.SetActive(false);
            text.text = "";
            goTextLingerTime.StopTimer();
        }
    }

    private void CountDownTimer()
    {
        if (!countDownTimer.GetIsGoing()) return;
        countDownTimer.CountDown(countDownMultiplier);
        // Show the proper message based on 100 ms intervals
        if (countDownTimer.Value <= 300 && countDownTimer.Value > 200)
        {
            text.text = "3";
        }
        else if (countDownTimer.Value <= 200 && countDownTimer.Value > 100)
        {
            text.text = "2";
        }
        else if (countDownTimer.Value <= 100 && countDownTimer.Value > 0)
        {
            text.text = "1";
        }
        else if (countDownTimer.Value <= 0)
        {
            text.text = "GO!";
        }

        if (countDownTimer.IsFinished())
        {
            gameStartEvent.Raise();
            goTextLingerTime.StartTimer();
            countDownTimer.StopTimer();

            //gameStartRaised = true;
        }
    }

    public void StartCountdown()
    {
        //Debug.Log("reset countdown");
        countDownTimer.StartTimer();
        //gameStartRaised = false;
        gameObject.SetActive(true); // Reactivate the game object if it was deactivated
    }

    public bool GetCountDownTimerFinished()
    {
        return countDownTimer.IsFinished();
    }
}
