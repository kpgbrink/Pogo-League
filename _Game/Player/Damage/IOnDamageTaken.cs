using UnityEngine;

public interface IOnDamageTaken
{
    void DamageTaken(
        float damagedAmount,
        Collision collision, 
        DamageGiver damageGiver, 
        DamageTaker damageTaker
        );
}
