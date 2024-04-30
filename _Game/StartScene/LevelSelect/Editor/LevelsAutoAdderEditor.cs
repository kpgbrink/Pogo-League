using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Linq;
using AutoLevelMenu.Common;
using AutoLevelMenu.EditorNS;

namespace AutoLevelMenu.LevelSelect
{
    [CustomEditor(typeof(LevelsAutoAdder))]
    public class LevelsAutoAdderEditor : Editor
    {
        LevelsAutoAdder levelsAutoAdder;
        LevelsData levelsData;
        List<WorldData> worldsData = new List<WorldData>();
        EditorLevelsData editorLevelsData;

        const string levelsDataFileName = "levelsData.asset";

        const string worldDataFolderName = "Data";
        const string worldDataFileName = "worldData.asset";

        void OnEnable()
        {
            levelsAutoAdder = target as LevelsAutoAdder;
            levelsData = levelsAutoAdder.levelsData;
            editorLevelsData = levelsAutoAdder.editorLevelsData;
            //ChangeFileNames();
            LoadWorldsData();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("editorLevelsData");
            var editorLevelsDataEditor = CreateEditor(editorLevelsData);
            editorLevelsDataEditor.DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                // Save to the editorLevelsData
                EditorUtility.SetDirty(editorLevelsData);
            }

            if (GUILayout.Button("Change Files and Generate World and Levels"))
            {
                ChangeFileNames();
                SetLevelScenes();
                EditorUtility.SetDirty(levelsData);
                EditorUtility.SetDirty(editorLevelsData);
                LoadWorldsData();
            }

            void FolderErrorMessage(string folderFromBase)
            {
                if (!Directory.Exists(Path.Combine(Global.baseFolder, folderFromBase)))
                {
                    Debug.LogError("The folder: " + Path.Combine(Global.baseFolder, folderFromBase) + " does not exist");
                }
            }

            FolderErrorMessage(levelsData.scenesFolder);
            FolderErrorMessage(levelsData.worldsFolder);

            // Show the user what the base folder is called
            EditorGUILayout.HelpBox("The base folder is: " + Global.baseFolder + "\nIf this is not correct edit the Global.cs file", UnityEditor.MessageType.Info);

            // Show error if the path is wrong
            var sceneLocationsProperties = typeof(SceneLocations).GetFields();
            foreach (var p in sceneLocationsProperties)
            {
                var sceneLocationString = p.GetValue(levelsData.sceneLocations) as string;
                if (Utils.PathStartsWithPath(sceneLocationString, levelsData.scenesFolder) || Utils.PathStartsWithPath(sceneLocationString, Path.Combine(Global.baseFolder, levelsData.scenesFolder)))
                {
                    Debug.LogError("You cannot include the path to the scenes folder in the scene locations");
                }
                var sceneFilePath = Path.Combine(Global.baseFolder, levelsData.scenesFolder, sceneLocationString) + Global.sceneFileType;
                if (!File.Exists(sceneFilePath))
                {
                    Debug.LogError("File at path: " + sceneFilePath + " does not exist");
                }
            }

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("levelsData");
            var levelsDataEditor = CreateEditor(levelsData);
            levelsDataEditor.DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(levelsData);
            }

            EditorGUILayout.LabelField("World Data", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            foreach (var worldData in worldsData)
            {
                GUI.SetNextControlName(worldData.worldName);
                EditorGUILayout.LabelField(worldData.worldName, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                var worldDataEditor = CreateEditor(worldData);
                worldDataEditor.DrawDefaultInspector();
                if (EditorGUI.EndChangeCheck())
                {
                    // Save to the editorLevelsData
                    EditorUtility.SetDirty(editorLevelsData);
                }
                EditorGUI.indentLevel--;
                EditorUtils.GuiLine();
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(8);
        }

        // Get the world folder name from a path
        string GetWorldFolderName(string scenePath)
        {
            if (!Path.HasExtension(scenePath))
            {
                throw new System.ArgumentException(scenePath + " must have file extension", "scenePath");
            }
            return Path.GetFileName(Path.GetDirectoryName(scenePath));
        }

        bool GrandParentDirectoryIsWorlds(string filePath)
        {
            return Path.GetFullPath(Path.Combine(Global.baseFolder, levelsData.worldsFolder)) == Path.GetFullPath(Path.GetDirectoryName(Path.GetDirectoryName(filePath)));
        }

        /// <summary>
        /// Reload data from each world folder
        /// </summary>
        void LoadWorldsData()
        {
            AssetDatabase.Refresh();

            worldsData.Clear();
            // Try loading the world colors
            // If I can't find the file then just set it to default
            var worldDirectories = Directory.EnumerateDirectories(Path.Combine(Global.baseFolder, levelsData.worldsFolder));
            foreach (var worldDirectory in worldDirectories)
            {
                // Create Data folder
                var worldDataDirectory = Path.Combine(worldDirectory, worldDataFolderName);
                Directory.CreateDirectory(worldDataDirectory);

                var worldDataAssetFilePath = Path.Combine(worldDataDirectory, worldDataFileName);
                var worldData = AssetDatabase.LoadAssetAtPath(worldDataAssetFilePath, typeof(WorldData)) as WorldData;
                if (worldData == null)
                {
                    // Make the world color exist
                    worldData = CreateInstance<WorldData>();
                    AssetDatabase.CreateAsset(worldData, worldDataAssetFilePath);
                }
                worldData.worldName = Path.GetFileName(worldDirectory);
                // Tell editor data changed
                EditorUtility.SetDirty(worldData);
                worldsData.Add(worldData);
            }
        }

        /// <summary>
        /// Change file names to have increasing numeric start
        /// </summary>
        void ChangeFileNames()
        {
            // Go through all world folders to make the names start with #. at 1 increasing
            var worldDirectories = Directory.EnumerateDirectories(Path.Combine(Global.baseFolder, levelsData.worldsFolder));
            worldDirectories = worldDirectories.OrderBy(NaturalSort.BuildComparable).ToList();
            var worldDirectoryCount = 0;
            foreach (var worldDirectory in worldDirectories)
            {
                worldDirectoryCount++;
                var worldFolderName = Path.GetFileName(worldDirectory);
                var worldFolderNameProperStart = worldDirectoryCount + ". ";
                worldFolderName = Utils.RemoveNumericStart(worldFolderName);
                worldFolderName = worldFolderNameProperStart + worldFolderName;
                var moveWorldDirectoryTo = Path.Combine(Global.baseFolder, levelsData.worldsFolder, worldFolderName);
                if (Path.GetFullPath(worldDirectory) != Path.GetFullPath(moveWorldDirectoryTo))
                {
                    Debug.Log(Path.GetFullPath(worldDirectory) + " >>> world transfer >>> " + Path.GetFullPath(moveWorldDirectoryTo));
                    Directory.Move(worldDirectory, moveWorldDirectoryTo);
                    // Also move the .meta file
                    File.Move(worldDirectory + EditorGlobal.unityMetaFileType, moveWorldDirectoryTo + EditorGlobal.unityMetaFileType);
                }
            }

            // Go through all world scenes and make sure the names start with #. at 1 increasing
            var filePaths = Directory.EnumerateFiles(Path.Combine(Global.baseFolder, levelsData.worldsFolder), "*" + Global.sceneFileType, SearchOption.AllDirectories);
            filePaths = filePaths.OrderBy(NaturalSort.BuildComparable).ToList();
            var levelSceneCount = 0;
            var worldString = "";
            foreach (var filePath in filePaths)
            {
                if (!GrandParentDirectoryIsWorlds(filePath))
                {
                    continue;
                }

                // If new world then set level SceneCount back to 1
                if (Path.GetFullPath(GetWorldFolderName(filePath)) != worldString)
                {
                    levelSceneCount = 0;
                    worldString = Path.GetFullPath(GetWorldFolderName(filePath));
                }

                levelSceneCount++;
                var sceneName = Path.GetFileNameWithoutExtension(filePath);
                var levelSceneNameProperStart = levelSceneCount + ". ";
                sceneName = Utils.RemoveNumericStart(sceneName);
                sceneName = levelSceneNameProperStart + sceneName;
                var fileStartingPath = Path.GetDirectoryName(filePath);
                var moveSceneFileTo = Path.Combine(fileStartingPath, sceneName) + Global.sceneFileType;
                if (Path.GetFullPath(filePath) != Path.GetFullPath(moveSceneFileTo))
                {
                    Debug.Log(Path.GetFullPath(filePath) + " >>> level transfer >>> " + Path.GetFullPath(moveSceneFileTo));
                    File.Move(filePath, moveSceneFileTo);
                    // Also move the .meta file
                    File.Move(filePath + EditorGlobal.unityMetaFileType, moveSceneFileTo + EditorGlobal.unityMetaFileType);
                }
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Add scenes to build. Change levels data. Add UI for the levels
        /// </summary>
        void SetLevelScenes()
        {
            // Clear List
            editorLevelsData.sceneAssets.Clear();

            var scenesPath = Path.Combine(Global.baseFolder, levelsData.scenesFolder);
            var worldsPath = Path.Combine(Global.baseFolder, levelsData.worldsFolder);

            // Get list of all dirs I want
            var scenes = Directory.EnumerateFiles(scenesPath, "*" + Global.sceneFileType, SearchOption.AllDirectories);

            // Add the scenes from the worlds directory of it does not start with scenes directory
            if (!Utils.PathStartsWithPath(worldsPath, scenesPath))
            {
                var worldScenes = Directory.EnumerateFiles(worldsPath, "*" + Global.sceneFileType, SearchOption.AllDirectories);
                scenes = scenes.Concat(worldScenes);
            }

            scenes = scenes.OrderBy(NaturalSort.BuildComparable).ToList();
            foreach (var scene in scenes)
            {
                var sceneAsset = (SceneAsset)AssetDatabase.LoadAssetAtPath(scene, typeof(SceneAsset));
                editorLevelsData.sceneAssets.Add(sceneAsset);
            }

            // Find valid Scene paths and make a list of EditorBuildSettingsScene
            var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            foreach (var sceneAsset in editorLevelsData.sceneAssets)
            {
                var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (!string.IsNullOrEmpty(scenePath))
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

            var childNotDelete = 0;
            // Destroy all children of listLayout first
            while (levelsAutoAdder.listLayout.transform.childCount != childNotDelete)
            {
                var child = levelsAutoAdder.listLayout.transform.GetChild(childNotDelete);
                // Do not delete if has do not delete script
                if (child.GetComponent<DoNotDelete>() != null)
                {
                    childNotDelete++;
                    continue;
                }
                DestroyImmediate(levelsAutoAdder.listLayout.transform.GetChild(childNotDelete).gameObject);
            }

            levelsData.worldsData = worldsData;
            levelsData.levelFiles.Clear();

            var levelNumber = 0;
            var worldCount = -1;
            var currentWorldString = "";
            GameObject currentWorldContent = null;
            foreach (var sceneAsset in editorLevelsData.sceneAssets)
            {
                var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                // Skip if not a level scene
                if (!GrandParentDirectoryIsWorlds(scenePath))
                {
                    continue;
                }

                levelsData.levelFiles.Add(scenePath);

                if (GetWorldFolderName(scenePath) != currentWorldString)
                {
                    levelNumber = 0;
                    worldCount++;
                    currentWorldString = GetWorldFolderName(scenePath);

                    // Make new world text
                    var worldTextPrefab = PrefabUtility.InstantiatePrefab(editorLevelsData.worldTextPrefab) as GameObject;
                    worldTextPrefab.transform.SetParent(levelsAutoAdder.listLayout.transform, false);
                    worldTextPrefab.name = currentWorldString;
                    worldTextPrefab.GetComponent<TextMeshProUGUI>().text = currentWorldString;
                    // Make new content container
                    currentWorldContent = PrefabUtility.InstantiatePrefab(editorLevelsData.worldContentPrefab) as GameObject;
                    currentWorldContent.transform.SetParent(levelsAutoAdder.listLayout.transform, false);
                    currentWorldContent.name = "Content";
                    currentWorldContent.GetComponent<Image>().color = levelsData.worldsData[worldCount].contentBackgroundColor;
                }

                levelNumber++;
                // Add a button to the level in the currently selected content container
                var levelButtonPrefab = PrefabUtility.InstantiatePrefab(editorLevelsData.levelButtonPrefab) as GameObject;
                levelButtonPrefab.transform.SetParent(currentWorldContent.transform, false);
                // Set button name and color
                levelButtonPrefab.name = Utils.GetSceneName(scenePath);
                levelButtonPrefab.GetComponent<Image>().color = levelsData.worldsData[worldCount].buttonColor;
                // Set the button texts
                var numberTextTransform = levelButtonPrefab.transform.Find("NumberText");
                numberTextTransform.GetComponent<TextMeshProUGUI>().text = levelNumber.ToString();
                var levelNameTransform = levelButtonPrefab.transform.Find("LevelNameText");
                levelNameTransform.GetComponent<TextMeshProUGUI>().text = Utils.GetSceneName(scenePath, true);
                // Set level button click method
                var levelButton = levelButtonPrefab.GetComponent<Button>();
                // Add change scene event
                var levelSelectAtion = new UnityAction<string>(levelsAutoAdder.levelSelector.Select);
                UnityEventTools.AddStringPersistentListener(levelButton.onClick, levelSelectAtion, scenePath);
            }
        }
    }
}