using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoLevelMenu.Common
{
    [System.Serializable]
    public class WorldData : ScriptableObject
    {
        //[HideInInspector]
        public string worldName = "";
        public Color buttonColor = Color.white;
        public Color contentBackgroundColor = Color.white;
    }
}
