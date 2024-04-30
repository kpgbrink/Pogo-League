using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamageGivenImpulse : MonoBehaviour, IOnDamageGiven
{
    public string testString;
    Rigidbody rb;
    public float impulseMultiplier = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void DamageGiven(
        float damagedAmount,
        Collision collision,
        DamageGiver damageGiver,
        DamageTaker damageTaker
        )
    {
        Debug.Log($"Given Impulse {testString}");
        rb.AddForce(collision.impulse * impulseMultiplier);
    }
}
