using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AutoLevelMenu.Common;

namespace AutoLevelMenu.LevelSelect
{
    [CreateAssetMenu(fileName = "EditorLevelsData" ,menuName = Global.AssetMenuPathStart.gameData + "/" + nameof(EditorLevelsData))]
    public class EditorLevelsData : ScriptableObject
    {
        public GameObject worldTextPrefab;
        public GameObject worldContentPrefab;
        public GameObject levelButtonPrefab;
#if UNITY_EDITOR
        public List<SceneAsset> sceneAssets = new List<SceneAsset>();
#endif
    }
}
