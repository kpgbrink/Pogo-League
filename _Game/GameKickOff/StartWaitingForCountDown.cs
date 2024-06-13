using AutoLevelMenu.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts._Game.GameStart
{
    public class StartWaitingForCountDown : MonoBehaviour
    {
        // This will be the four seconds for the thing
        CountDownTimer countDownTimer = new(300);

        [SerializeField]
        int multiplyCountDown = 1;

        [SerializeField]
        GameEvent gameStartCountdownTimerEvent;

        private void Start()
        {
            countDownTimer.StartTimer();
        }

        private void FixedUpdate()
        {
            countDownTimer.CountDown(multiplyCountDown);
            if (countDownTimer.CheckFinished())
            {
                Debug.Log("countdown finished");
                gameObject.SetActive(false);
                gameStartCountdownTimerEvent.Raise();
            }
        }

        public void ResetCountDown()
        {
            countDownTimer.StartTimer();
            gameObject.SetActive(true);
        }
    }
}
