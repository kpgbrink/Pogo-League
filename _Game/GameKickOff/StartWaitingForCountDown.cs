using AutoLevelMenu.Events;
using UnityEngine;

namespace Assets.Scripts._Game.GameStart
{
    public class StartWaitingForCountDown : MonoBehaviour
    {
        // This will be the four seconds for the thing
        CountDownTimer countDownTimer = new(100);

        [SerializeField]
        int multiplyCountDown = 1;

        [SerializeField]
        GameEvent startCountdownTimerEvent;

        private void Start()
        {
            countDownTimer.StartTimer();
        }

        private void FixedUpdate()
        {
            countDownTimer.CountDown(multiplyCountDown);
            if (countDownTimer.IsFinished())
            {
                startCountdownTimerEvent.Raise();
                gameObject.SetActive(false);
            }
        }

        public void StartCountdown()
        {
            countDownTimer.StartTimer();
            gameObject.SetActive(true);
        }
    }
}
