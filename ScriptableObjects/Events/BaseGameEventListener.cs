using UnityEngine;

namespace AutoLevelMenu.Events
{
    public interface IGameEventListener<TResponse>
    {
        TResponse TheResponse { get; }
    }
    public abstract class BaseGameEventListener<TResponse, TEvent> : MonoBehaviour, IGameEventListener<TResponse>
        where TEvent : GameEvent<TResponse>
    {
        [Tooltip("Event to register with.")]
        public TEvent Event;

        public abstract TResponse TheResponse { get; }

        void OnEnable()
        {
            Event.RegisterListener(this);
        }

        void OnDisable()
        {
            Event.UnregisterListener(this);
        }
    }
}
