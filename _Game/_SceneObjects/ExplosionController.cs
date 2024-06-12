using UnityEngine;
using UnityEngine.VFX;

public class ExplosionController : MonoBehaviour
{
    private VisualEffect visualEffect;

    private void Awake()
    {
        visualEffect = GetComponentInChildren<VisualEffect>(); // Make sure this matches the actual hierarchy
    }

    public void ActivateExplosion(Vector3 position, Quaternion rotation, float goalSpeed)
    {
        transform.SetPositionAndRotation(position, rotation);

        // Set the GoalSpeed property
        visualEffect.SetFloat("GoalSpeed", goalSpeed);

        // Activate and play the visual effect
        gameObject.SetActive(true);
        visualEffect.Play();

        // Schedule deactivation
    }

}
