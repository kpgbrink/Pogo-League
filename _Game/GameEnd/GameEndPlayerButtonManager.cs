using AutoLevelMenu.Common;
using AutoLevelMenu.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameEndPlayerButtonManager : MonoBehaviour
{
    [SerializeField]
    LevelsData levelsData;

    [SerializeField]
    PlayerSparData playerSparData;

    Transform OneToCopy => transform.GetChild(0);

    [SerializeField]
    StringGameEvent normalLoadEvent;

    List<GameEndPlayerButton> GameEndPlayerButtons = new List<GameEndPlayerButton>();

    [SerializeField]
    CountDownTimer leaveSceneTimer;

    void Start()
    {
        AddPlayerButtons();
    }

    void AddPlayerButtons()
    {
        var toCopy = OneToCopy;
        playerSparData.Players.ForEach(player =>
        {
            // Create
            var obj = Object.Instantiate(toCopy, transform);
            player.AddPlayerControlledGameObject(obj);
            obj.gameObject.SetActive(true);
            GameEndPlayerButtons.Add(obj.GetComponent<GameEndPlayerButton>());
        });
        toCopy.gameObject.SetActive(false);
    }

    private void Update()
    {
        LeaveSceneTimer();
    }

    private void LeaveSceneTimer()
    {
        if (GameEndPlayerButtons.Count == 0) return;
        // If not all on last thing then reset the timer value to start
        if (GameEndPlayerButtons.Find(o => o.LastChildActive == false) != null)
        {
            leaveSceneTimer.Value = leaveSceneTimer.StartValue;
        }
        else
        {
            leaveSceneTimer.CountDownTimeDelta();
            if (leaveSceneTimer.CheckFinished())
            {
                // Leave the scene
                normalLoadEvent.Raise(levelsData.SceneLocationFullPath(levelsData.sceneLocations.mainMenu));
            }
        }
    }
}
