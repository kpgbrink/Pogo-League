using UnityEngine;

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
            if (maxTimer.IsFinished())
            {
                ObjectPooler.Release(gameObject);
            }
        }
    }
}
