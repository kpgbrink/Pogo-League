using System;
using UnityEngine;

[Serializable]
public class CountDownTimer
{
    [SerializeField]
    float startValue = 60;
    public float StartValue { get => startValue; set => startValue = value; }

    public float Value { get; set; } = 0;

    bool Going { get; set; } = true;

    public CountDownTimer() { }

    public CountDownTimer(float startValue)
    {
        StartValue = startValue;
    }

    public void CountDown()
    {
        if (!ShouldCountDown) return;
        Value--;
    }

    public void CountDown(float delta)
    {
        if (!ShouldCountDown) return;
        Value -= delta;
    }

    public void CountDownTimeDelta()
    {
        if (!ShouldCountDown) return;
        Value -= Time.deltaTime;
    }

    public bool GetIsGoing() => Going;


    bool ShouldCountDown => Value > 0 && Going;

    public void StartTimer()
    {
        Value = StartValue;
        Going = true;
    }

    public void ResumeTimer()
    {
        Going = true;
    }

    public void StopTimer()
    {
        Going = false;
    }

    // Check if the timer just finished
    public bool JustFinished(bool restartTimer = false)
    {
        if (Value > 0) return false;
        if (restartTimer)
            StartTimer();
        return true;
    }

    // Check if the timer is stopped
    public bool IsFinished(bool restartTimer = false)
    {
        if (Value > 0) return false;
        if (!Going) return false;
        if (restartTimer)
            StartTimer();
        return true;
    }
}
