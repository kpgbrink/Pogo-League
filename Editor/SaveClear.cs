using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AutoLevelMenu.Common;

namespace AutoLevelMenu.EditorNS
{
    public class SaveClear : ScriptableObject
    {
        [MenuItem(EditorGlobal.menuItemStart + "/SaveData/Delete all Save data")]
        static void DeleteAllSaveData()
        {
            if (EditorUtility.DisplayDialog("Delete all Save Data.",
                "Are you sure you want to delete all the save data? " +
                "This action cannot be undone.", "Yes", "No"))
            {
                Debug.Log("Deleting Save Data");
                GameControl.DeleteLevelsSaveData();
            }
        }
    }
}
