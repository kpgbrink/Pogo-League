using AutoLevelMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    public TextMeshPro tmp;

    public TextMeshProUGUI tmpUGUI;

    public void SetText(int Time)
    {
        var timeInSeconds = Time / Global.FixedTimeStep;
        var timeSpan = TimeSpan.FromSeconds(timeInSeconds);

        var text = timeSpan.ToString(@"mm\:ss");
        if (tmp != null)
        {
            tmp.text = text;
        }
        if (tmpUGUI != null)
        {
            tmpUGUI.text = text;
        }
    }
}
