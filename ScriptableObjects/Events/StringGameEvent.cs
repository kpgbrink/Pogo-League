using System;
using UnityEngine;

namespace AutoLevelMenu.Events
{
    [CreateAssetMenu(menuName = GameEvent.eventAssetMenuStart + "/" + nameof(StringGameEvent))]
    public class StringGameEvent : GameEvent<Action<string>>
    {
        public void Raise(string arg1)
        {
            RaiseAll(response => response(arg1));
        }
    }
}
