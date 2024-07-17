using AutoLevelMenu;
using System;
using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    public TextMeshPro tmp;

    public TextMeshProUGUI tmpUGUI;

    public void SetText(int Time)
    {
        var timeInSeconds = (float)Time / (float)Global.FixedTimeStep;
        var roundedTimeInSeconds = Math.Ceiling(timeInSeconds);
        var timeSpan = TimeSpan.FromSeconds(roundedTimeInSeconds);

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
