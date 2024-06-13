using UnityEngine;

namespace AutoLevelMenu.Events
{
    public class PlayerExplosionEventListener : Vector3FloatGameEventListener
    {
        [SerializeField]
        private Rigidbody rb;

        [SerializeField]
        private float explosionRadius = 15;

        [SerializeField]
        private float explosionAdjustForce = 0.4f;

        [SerializeField]
        private float minimumExplosionForce = 25f; // Minimum explosion force

        [SerializeField]
        private float minimumForcePercentage = 0.05f; // Minimum force as a percentage of adjusted explosion force

        protected override void Handle(Vector3 explosionPosition, float explosionForce)
        {
            Debug.Log("PlayerExplosionEventListener.Handle()");
            var direction = rb.position - explosionPosition;
            var distance = direction.magnitude;

            // Ensure the explosion force has a minimum value
            explosionForce = Mathf.Max(minimumExplosionForce, explosionForce);

            // Adjust the explosion force to matter only a little
            var adjustedExplosionForce = explosionForce * explosionAdjustForce; // Reduce the overall force by 60%

            // Calculate the scaled force based on distance with a linear fall-off
            var scaledForce = adjustedExplosionForce * (1 - (distance / explosionRadius));

            // Ensure a minimum force of a certain percentage of the adjusted explosion force
            var minimumForce = minimumForcePercentage * adjustedExplosionForce;
            scaledForce = Mathf.Max(minimumForce, scaledForce);

            // Apply the force to the rigidbody
            rb.AddForce(direction.normalized * scaledForce, ForceMode.Impulse);
        }
    }
}
