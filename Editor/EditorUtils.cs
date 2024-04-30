using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoLevelMenu.EditorNS
{
    public static class EditorUtils
    {
        public static void GuiLine(int i_height = 2)
        {
            var rect = EditorGUILayout.GetControlRect(false, i_height);

            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}

