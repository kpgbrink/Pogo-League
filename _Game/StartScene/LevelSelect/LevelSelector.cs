using AutoLevelMenu.Common;
using AutoLevelMenu.Events;
using UnityEngine;
using UnityEngine.UI;

namespace AutoLevelMenu.LevelSelect
{
    public class LevelSelector : MonoBehaviour
    {
        public GameControl gameControl;
        public GameObject listLayout;
        Button[] levelButtons;
        public StringGameEvent normalLoadEvent;
        public StringGameEvent progressLoadEvent;

        void Start()
        {
            // Set level buttons
            levelButtons = listLayout.GetComponentsInChildren<Button>();
        }

        void Update()
        {
        }

        public void Select(string levelName)
        {
            progressLoadEvent.Raise(levelName);
        }
    }
}
