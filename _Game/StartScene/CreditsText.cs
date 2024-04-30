using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AutoLevelMenu.MainMenuNS
{
    public class CreditsText : MonoBehaviour
    {
        public TextAsset creditsTextAsset;

        void Awake()
        {
            GetComponent<TextMeshProUGUI>().text = creditsTextAsset.text;
        }
    }
}
