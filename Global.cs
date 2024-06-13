using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AutoLevelMenu
{
    public static class Global
    {
        public const string baseFolder = "Assets/";

        public const string sceneFileType = ".unity";

        public static class AssetMenuPathStart
        {
            public const string gameData = "GameData";
            public const string scriptableObjectEnum = "Enum";
        }

        public static int FixedTimeStep = 100;
    }
}
