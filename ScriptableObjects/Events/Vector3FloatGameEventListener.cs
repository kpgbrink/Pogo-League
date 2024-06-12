using System;
using UnityEngine;

namespace AutoLevelMenu.Events
{
    public abstract class Vector3FloatGameEventListener : BaseGameEventListener<Action<Vector3, float>, Vector3FloatGameEvent>
    {
        public override Action<Vector3, float> TheResponse
        {
            get
            {
                return Handle;
            }
        }

        protected abstract void Handle(Vector3 position, float value);
    }
}