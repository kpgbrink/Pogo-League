using Assets.Scripts;
using AutoLevelMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Respawner : PlayerObject
{
    [SerializeField]
    ObjTeamList respawnPlanes;

    TransformData Transform;

    [SerializeField]
    UnityEvent respawnEvent;

    [SerializeField]
    bool useBasicRespawn = true;

    [SerializeField]
    bool respawnOnce = false;

    int RespawnCount { get; set; } = 0;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        respawnPlanes.Init();
        Transform = new TransformData(transform);
        rb = GetComponent<Rigidbody>();
    }

    public void Respawn()
    {
        if (respawnOnce && RespawnCount > 0)
        {
            return;
        }
        respawnEvent.Invoke();
        
        Debug.Log("Respawn");
        SimpleRespawn();
        RespawnCount++;
    }

    private void SimpleRespawn()
    {
        if (!useBasicRespawn) return;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        Debug.Log("Simple respawn");
        transform.position = Transform.Position;
        transform.localScale = Transform.Scale;
        transform.rotation = Transform.Rotation;
    }

    int? Team()
    {
        if (PlayerSpar == null) return null;
        //Debug.Log($"Team {PlayerSpar.Team}");
        return PlayerSpar.Team;
     }

    public void CheckPastPlane()
    {

        foreach (var respawnPlane in respawnPlanes.FilteredTransformList(Team()))
        {
            if (Utils.CheckPastPlane(respawnPlane, transform))
            {
                Respawn();
            }
        }
    }

    private void FixedUpdate()
    {
        CheckPastPlane();
    }

    public void RemoveTeamObjects(int team)
    {
        respawnPlanes.FilteredTransformList(team);
    }

    // Generate thing that adds two values and divides by 3

}
