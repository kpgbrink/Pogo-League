using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AutoLevelMenu.EditorNS
{
    // https://docs.unity3d.com/ScriptReference/EditorPrefs.DeleteAll.html
    public class PlayerPreferencesClear : ScriptableObject
    {
        [MenuItem(EditorGlobal.menuItemStart + "/PlayerPrefs/Delete all Player Prefs")]
        static void DeleteAllPlayerPrefs()
        {
            if (EditorUtility.DisplayDialog("Delete all Player Prefs.",
                "Are you sure you want to delete all the player preferences? " +
                "This action cannot be undone.", "Yes", "No"))
            {
                Debug.Log("Deleting Player Preferences");
                PlayerPrefs.DeleteAll();
            }
        }
    }
}
