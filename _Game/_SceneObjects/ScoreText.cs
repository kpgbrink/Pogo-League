using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public int teamNumber;

    public int playerNumber;

    public TextMeshPro tmp;

    public TextMeshProUGUI tmpUGUI;

    public void SetText(string text)
    {
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
