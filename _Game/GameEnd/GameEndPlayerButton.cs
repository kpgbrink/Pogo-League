using Assets.Scripts;
using AutoLevelMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameEndPlayerButton : PlayerInputBase
{
    GameEndPlayerButtonManager GameEndPlayerButtonManager { get; set; }

    public bool? LastChildActive
    {
        get
        {
            var lastChildTransform = transform.GetChild(transform.childCount - 1);
            if (lastChildTransform == null) return null;
            return lastChildTransform.gameObject.activeSelf;
        }
    }

    interface ITextSetter
    {
        void SetStrings(GameEndPlayerButton gameEndPlayerButton);
    }

    Player GetPlayer()
    {
        if (PlayerInputTransform == null) return null;
        var player = PlayerInputTransform.GetComponent<Player>();
        return player;
    }

    PlayerSpar GetPlayerSpar()
    {
        if (PlayerInputTransform == null) return null;
        var playerSpar = PlayerInputTransform.GetComponent<PlayerSpar>();
        return playerSpar;
    }

    // First panel
    [Serializable]
    class Placement : ITextSetter
    {
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI placementOfPlayer;

        public virtual void SetStrings(GameEndPlayerButton gameEndPlayerButton)
        {
            var player = gameEndPlayerButton.GetPlayer();
            if (player == null) return;
            var playerSpar = gameEndPlayerButton.GetPlayerSpar();
            playerName.text = player.PlayerName;

            // Calculate placement of player
            var playerSparManager = player.playerSparData;
            var orderByDeathSelector = FuncUtil.Infer((int? deathTime) => (deathTime.HasValue ? 1 : 0, -deathTime));
            var playersAndPlayerSpars = playerSparManager.Players.Zip(playerSparManager.PlayerSpars, (myPlayer, myPlayerSpar) => (player: myPlayer, playerSpar: myPlayerSpar)).ToList();
            var bestFixedUpdateTimeByTeam = playersAndPlayerSpars.Where(ps => ps.playerSpar.Team != 0).GroupBy(ps => ps.playerSpar.Team).ToDictionary(psGroup => psGroup.Key, psGroup => psGroup.Select(x => x.player.PlayerDeathFixedUpdateTime).OrderBy(orderByDeathSelector).First());
            var zeroBasedPlacement = playersAndPlayerSpars.Select(x =>
            {
                return (x.player, bestFixedUpdateTime: x.playerSpar.Team == 0 ? x.player.PlayerDeathFixedUpdateTime : bestFixedUpdateTimeByTeam[x.playerSpar.Team]);
            }).OrderBy(x => orderByDeathSelector(x.bestFixedUpdateTime)).Select((x, index) => (index, x.player)).First(x => x.player == player).index;

            placementOfPlayer.text = $"{Utils.AddOrdinal(zeroBasedPlacement + 1)} place";
        }
    }

    [SerializeField]
    Placement placement;

    [Serializable]
    class Stats : Placement
    {
        public TextMeshProUGUI killHistory;
        public TextMeshProUGUI kills;
        public TextMeshProUGUI deaths;
        public TextMeshProUGUI damageDone;
        public TextMeshProUGUI damageTaken;
        public TextMeshProUGUI surviveTime;

        public override void SetStrings(GameEndPlayerButton gameEndPlayerButton)
        {
            base.SetStrings(gameEndPlayerButton);
            var player = gameEndPlayerButton.GetPlayer();
            if (player == null) return;
            var playerSpar = gameEndPlayerButton.GetPlayerSpar();

            killHistory.text = FuncUtil.Invoke(() =>
            {
                var text = "Kill History: ";
                player.ResetValuesOnSceneInit.KillsDone.ForEach(o => text = $"{text} {o.playerNumber}");
                return text;
            });
            kills.text = $"Kills: {player.ResetValuesOnSceneInit.KillsDone.Count}";
            deaths.text = $"Deaths: {player.ResetValuesOnSceneInit.DeathsDone.Count}";
            damageDone.text = $"Damage Given: {player.ResetValuesOnSceneInit.DamagesGiven.Sum(o => o.Value)}";
            damageTaken.text = $"Damage Taken:{player.ResetValuesOnSceneInit.DamagesTaken.Sum(o => o.Value)}";
            string surviveTimeText()
            {
                if (player.ResetValuesOnSceneInit.DeathFinalTime == null)
                {
                    return "Forever";
                }
                return $"{player.ResetValuesOnSceneInit.DeathFinalTime}";
            }
            surviveTime.text = $"Survive Until: {surviveTimeText()}";
        }
    }
    [SerializeField]
    Stats stats;

    [Serializable]
    class Finished : ITextSetter
    {
        public TextMeshProUGUI playerName;

        public virtual void SetStrings(GameEndPlayerButton gameEndPlayerButton)
        {
            var player = gameEndPlayerButton.GetPlayer();
            if (player == null) return;
            var playerSpar = gameEndPlayerButton.GetPlayerSpar();
            playerName.text = player.PlayerName;
        }
    }
    [SerializeField]
    Finished finished;

    void SetAllData()
    {
        var textSetters = new ITextSetter[] { placement, stats, finished };
        textSetters.ToList().ForEach(t => t.SetStrings(this));
    }

    void Start()
    {
        // Set only first child as active
        SetAllChildrenInactive();
        transform.GetChild(0).gameObject.SetActive(true);
        SetAllData();
    }

    void SetAllChildrenInactive()
    {
        // Set all but the first as active
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Go to next
    void GoToNext()
    {
        var newActive = Math.Min(GetCurrentActiveIndex() + 1, transform.childCount - 1);
        SetNewActive(newActive);
    }

    void GoBack()
    {
        var newActive = Math.Max(GetCurrentActiveIndex() - 1, 0);
        SetNewActive(newActive);
    }

    int GetCurrentActiveIndex()
    {
        var index = -1;
        foreach (Transform child in transform)
        {
            index++;
            if (child.gameObject.activeSelf)
            {
                //var newActive = Mathf.Min(transform.childCount - 1, index++);
                return index;
            }
        }
        return 0;
    }

    void SetNewActive(int newActive)
    {
        var index = -1;
        foreach (Transform child in transform)
        {
            index++;
            child.gameObject.SetActive(index == newActive);
        }
    }

    public override void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
        if (inputAction.action.phase == InputActionPhase.Performed)
        {
            switch (inputAction.action.name)
            {
                case PlayerInputs.ButtonEast:
                    // Go backwards
                    //inputAction.action.
                    GoBack();
                    break;
                case PlayerInputs.ButtonSouth:
                    // Go through forwards
                    GoToNext();
                    break;
            }
        }
    }
}
