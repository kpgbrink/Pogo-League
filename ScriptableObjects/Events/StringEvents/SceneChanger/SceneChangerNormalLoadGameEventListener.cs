using AutoLevelMenu.Common;
using UnityEngine;
using UnityEngine.UI;

namespace AutoLevelMenu.Events
{
    public class SceneChangerNormalLoadGameEventListener : StringGameEventListener
    {
        public SceneChanger sceneChanger;

        protected override void Handle(string arg1)
        {
            if (arg1 == string.Empty)
            {
                sceneChanger.NormalLoad();
            }
            else
            {
                sceneChanger.NormalLoad(arg1);
            }
        }
    }
}