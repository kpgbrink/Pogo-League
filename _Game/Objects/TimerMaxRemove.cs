using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts._Game.Objects
{
    public class TimerMaxRemove : MonoBehaviour, IPooledObject
    {
        public CountDownTimer maxTimer;
        public ObjectPooler ObjectPooler { get; set; }

        public void OnEnable()
        {
            maxTimer.StartTimer();
        }

        public void FixedUpdate()
        {
            maxTimer.CountDown();
            if (maxTimer.CheckFinished())
            {
                ObjectPooler.Release(gameObject);
            }
        }
    }
}
