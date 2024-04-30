using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SimpleColorIndication : MonoBehaviour
{
    [SerializeField]
    PlayerInputBase playerInputBase;

    PlayerSpar playerSpar;

    void Start()
    {
        playerSpar = playerInputBase.PlayerInputTransform.GetComponent<PlayerSpar>();
        SetColor();
    }

    void SetColor()
    {
        if (playerSpar == null) return;
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.color = playerSpar.TeamColor;
    }
}
