using AutoLevelMenu.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoLevelMenu.LevelSelect
{
    public class LevelsAutoAdder : MonoBehaviour
    {
#if UNITY_EDITOR
        public LevelSelector levelSelector;
        public GameObject listLayout;
        public LevelsData levelsData;
        public EditorLevelsData editorLevelsData;
#endif
    }
}
