using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DamageImpulse : MonoBehaviour, IOnDamageTaken
{
    public string testString;
    Rigidbody rb;
    public float impulseMultiplier = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void DamageTaken(
        float damagedAmount, 
        Collision collision,
        DamageGiver damageGiver, 
        DamageTaker damageTaker
        )
    {
        Debug.Log($"Damage Impulse {testString}");
        rb.AddForce(collision.impulse * impulseMultiplier);
    }
}
