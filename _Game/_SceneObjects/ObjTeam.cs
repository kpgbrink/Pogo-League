using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTeam : MonoBehaviour
{
    [SerializeField]
    int teamNumber;
    public int TeamNumber { get => teamNumber; set => teamNumber = value; }
}
