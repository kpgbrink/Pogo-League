using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPlayerObject 
{
    Transform PlayerInputTransform { get; set; }
    Player Player { get; set; }
    PlayerSpar PlayerSpar { get; set; }
}

public class PlayerObject : MonoBehaviour, IPlayerObject
{
    protected Transform playerInputTransform;
    public Player Player { get; set; }
    public PlayerSpar PlayerSpar { get; set; }
    public Transform PlayerInputTransform
    {
        get => playerInputTransform;
        set
        {
            playerInputTransform = value;
            Player = value.GetComponent<Player>();
            PlayerSpar = value.GetComponent<PlayerSpar>();
        }
    }
}
