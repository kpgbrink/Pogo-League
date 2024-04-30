using System;
using System.Collections.Generic;

namespace AutoLevelMenu.Common
{
    [Serializable]
    public class LevelsSaveData
    {
        public string currentLevel = "";
        [Serializable]
        public class LevelsData : SerializableDictionary<string, LevelSaveData> { };
        public LevelsData levelsData = new LevelsData();
    }
}