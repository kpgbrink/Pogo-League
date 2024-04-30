using Assets.Scripts._Game.PlayerManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains values for things that only matter with pvp stuff
/// </summary>
public class PlayerSpar : MonoBehaviour
{
    public static readonly int maxTeams = 2;
    // 0 is no team
    int chosenTeam;
    public int ChoosenTeam
    {
        get => chosenTeam;
        set
        {
            // Wrap around
            if (value < 0)
            {
                chosenTeam = maxTeams;
                return;
            }
            if (value > maxTeams)
            {
                chosenTeam = 0;
                return;
            }
            Team = value;
            chosenTeam = value;
            Debug.Log("Chosen team changed to," + value);
        }
    }

    /// <summary>
    /// This is set by the spawn manager if it needs to be changed
    /// 0 is no team.
    /// </summary>
    public int Team { get; set; }

    [SerializeField]
    PlayerTeamColors playerTeamColors;
    public PlayerTeamColors PlayerTeamColors { get => playerTeamColors; }

    public Color TeamColor => PlayerTeamColors.teamColors[Team];

    // Resets on scene change
    public class ResetableValues
    {
        public int Wins { get; set; }
        public int Loses { get; set; }
    }
    public ResetableValues ResetValues { get; set; }

    private void Start()
    {
        PlayerSparSceneInitialize();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    void ChangedActiveScene(Scene current, Scene next)
    {
        PlayerSparSceneInitialize();
    }

    public void PlayerSparSceneInitialize()
    {
        ResetValues = new ResetableValues();
    }
}
