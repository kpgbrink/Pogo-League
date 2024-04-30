using System;

namespace AutoLevelMenu.Events
{
    public abstract class StringGameEventListener : BaseGameEventListener<Action<string>, StringGameEvent>
    {
        public override Action<string> TheResponse
        {
            get
            {
                return Handle;
            }
        }

        protected abstract void Handle(string arg1);
    }
}
