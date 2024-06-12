using System;
using UnityEngine;

namespace AutoLevelMenu.Events
{
    [CreateAssetMenu(menuName = GameEvent.eventAssetMenuStart + "/" + nameof(Vector3FloatGameEvent))]
    public class Vector3FloatGameEvent : GameEvent<Action<Vector3, float>>
    {
        public void Raise(Vector3 position, float value)
        {
            RaiseAll(response => response(position, value));
        }
    }
}