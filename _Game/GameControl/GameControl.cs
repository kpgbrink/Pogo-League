using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AutoLevelMenu.Common
{
    [CreateAssetMenu(fileName = nameof(GameControl), menuName = Global.AssetMenuPathStart.gameData + "/" + nameof(GameControl))]
    public class GameControl : ScriptableObject
    {
        public LevelsData levelsData;
        LevelsSaveData levelsSaveData = new LevelsSaveData();
        public LevelsSaveData LevelsSaveData
        {
            get
            {
                LoadLevelsSaveData();
                return levelsSaveData;
            }
            set
            {
                levelsSaveData = value;
                SaveLevelsSaveData();
            }
        }
        static readonly string saveDataFileName = "SaveSparData.txt";
        string levelsSaveDataPath;

        public void Awake()
        {
            levelsSaveDataPath = Path.Combine(Application.persistentDataPath, saveDataFileName);
        }

        public string GetCurrentLevel()
        {
            levelsSaveData = LevelsSaveData;
            // First try to use the currentLevel string
            if (levelsSaveData.currentLevel != null && levelsData.levelFiles.Exists(levelFile => levelFile == levelsSaveData.currentLevel))
            {
                return levelsSaveData.currentLevel;
            }

            // If current level string fails then just try to find the right level based on which are completed
            foreach (var levelFile in levelsData.levelFiles)
            {
                if (levelsSaveData.levelsData.ContainsKey(levelFile) && levelsSaveData.levelsData[levelFile].completed)
                {
                    continue;
                }
                return levelFile;
            }
            // If last level is completed go back to first
            return levelsData.levelFiles[0];
        }

        void SaveLevelsSaveData()
        {
            SaveData<LevelsSaveData>(levelsSaveData, levelsSaveDataPath);
        }

        void LoadLevelsSaveData()
        {
            levelsSaveData = LoadData<LevelsSaveData>(levelsSaveDataPath);
        }

        public static void DeleteLevelsSaveData()
        {
            // Delete levels save data
            var path = Path.Combine(Application.persistentDataPath, saveDataFileName);
            Debug.Log(path);
            File.Delete(path);
        }

        static void SaveData<T>(T value, string path)
        {
            var jsonString = JsonUtility.ToJson(value);

            using (var streamWriter = File.CreateText(path))
            {
                streamWriter.Write(jsonString);
            }
        }

        static T LoadData<T>(string path)
            where T : new()
        {
            if (File.Exists(path))
            {
                using (var streamReader = File.OpenText(path))
                {
                    var jsonString = streamReader.ReadToEnd();
                    return JsonUtility.FromJson<T>(jsonString);
                }
            }
            else
            {
                return new T();
            }
        }
    }
}