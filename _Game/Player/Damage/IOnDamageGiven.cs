using UnityEngine;

public interface IOnDamageGiven
{
    void DamageGiven(
        float damagedAmount,
        Collision collision,
        DamageGiver damageGiver,
        DamageTaker damageTaker
        );
}
