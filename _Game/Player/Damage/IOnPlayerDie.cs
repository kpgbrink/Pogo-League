using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnPlayerDie
{
    void OnPlayerDie(
        float? damagedAmount = null,
        Collision collision = null,
        DamageGiver damageGiver = null,
        DamageTaker damageTaker = null
        );
}
