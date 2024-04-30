using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes something move constantly.
/// Example Use: Hocky Puck makes it move if the velocity is more than one to prevent
/// getting stuck in the corner.
/// </summary>
public class ConstantMoving : MonoBehaviour, IOnDamageTaken
{
    [SerializeField]
    bool makeMovingOnDamage = false;

    [SerializeField]
    bool makeMoving = false;

    [SerializeField]
    float movingMinVel = 1;

    [SerializeField]
    Rigidbody rb;

    public void DamageTaken(float damagedAmount, Collision collision, DamageGiver damageGiver, DamageTaker damageTaker)
    {
        if (!makeMovingOnDamage) return;
        makeMoving = true;
    }

    public void SetConstantMoving(bool set)
    {
        makeMoving = set;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            ForceMove();
        }
    }

    private void ForceMove()
    {
        if (!makeMoving) return;
        // figure out current movement
        if (rb.linearVelocity.magnitude == 0) return;
        if (rb.linearVelocity.magnitude > movingMinVel) return;
        rb.linearVelocity = rb.linearVelocity.normalized * movingMinVel;
    }
}
