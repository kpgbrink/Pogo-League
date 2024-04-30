using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoLevelMenu.Common
{
    [CreateAssetMenu(fileName = "LevelsData", menuName = Global.AssetMenuPathStart.gameData + "/LevelsData")]
    public class LevelsData : ScriptableObject
    {
        public string scenesFolder = "Scenes";
        public string worldsFolder = "Scenes/SparZones";
        public SceneLocations sceneLocations;
        public List<WorldData> worldsData = new List<WorldData>();
        public List<string> levelFiles = new List<string>();

        public string SceneLocationFullPath(string sceneLocation)
        {
            // Get the full file path from the scene path
            return Path.Combine(Global.baseFolder, scenesFolder, sceneLocation) + Global.sceneFileType;
        }

        public WorldData FindWorldData(string scenePath)
        {
            return worldsData.Find(wC =>
            {
                return Utils.PathStartsWithPath(scenePath, Path.Combine(Global.baseFolder, worldsFolder, wC.worldName));
            });
        }

        public bool IsLevelScene(string scenePath)
        {
            return levelFiles.Find(levelFile => levelFile == scenePath) != null;
        }

        public bool IsLastLevel()
        {
            var scenePath = SceneManager.GetActiveScene().path;
            return scenePath == levelFiles[levelFiles.Count - 1];
        }
    }
}
