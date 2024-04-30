using UnityEngine;
using System.Collections;
using AutoLevelMenu.Common;

namespace AutoLevelMenu.Events
{
    public class SceneChangerProgressLoadGameEventListener : StringGameEventListener
    {
        public SceneChanger sceneChanger;

        protected override void Handle(string arg1)
        {
            if (arg1 == string.Empty)
            {
                sceneChanger.ProgressLoad();
            }
            else
            {
                sceneChanger.ProgressLoad(arg1);
            }
        }
    }
}