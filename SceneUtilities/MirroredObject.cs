using UnityEngine;

public class MirroredObject : MonoBehaviour
{
    [SerializeField] private GameObject mirrorTarget;  // The target GameObject to mirror
    public bool mirrorRotation = true;  // Should rotation be mirrored?
    public bool mirrorPosition = true;  // Should position be mirrored?
    public bool mirrorScale = true;

    public enum Axis { X, Y, Z }  // Axes available for mirroring
    public Axis mirrorAxis = Axis.X;  // Default axis to mirror around

    // Property to manage mirror target with synchronization
    public GameObject MirrorTarget
    {
        get => mirrorTarget;
        set
        {
            SetMirrorTarget(value, true);
        }
    }

    // Method to set the mirror target and manage the bidirectional link
    private void SetMirrorTarget(GameObject value, bool updateOther)
    {
        if (mirrorTarget == value) return;

        if (mirrorTarget != null)
        {
            // Detach old target if it is linked to this object
            var oldTargetComponent = mirrorTarget.GetComponent<MirroredObject>();
            if (oldTargetComponent != null && oldTargetComponent.MirrorTarget == this.gameObject)
            {
                oldTargetComponent.SetMirrorTarget(null, false);  // Break old link
            }
        }

        mirrorTarget = value;

        if (mirrorTarget != null && updateOther)
        {
            // Attach new target
            var targetComponent = mirrorTarget.GetComponent<MirroredObject>();
            if (targetComponent != null && targetComponent.MirrorTarget != this.gameObject)
            {
                targetComponent.SetMirrorTarget(this.gameObject, false);  // Create new link
            }
        }
    }

    // Call this method to synchronize settings and update the mirror
    public void UpdateLinkedMirror()
    {
        if (mirrorTarget != null)
        {
            var mirroredComponent = mirrorTarget.GetComponent<MirroredObject>();
            if (mirroredComponent != null)
            {
                mirroredComponent.mirrorRotation = this.mirrorRotation;
                mirroredComponent.mirrorPosition = this.mirrorPosition;
                mirroredComponent.mirrorAxis = this.mirrorAxis;
                mirroredComponent.UpdateMirror(); // Force update if needed
            }
        }
    }

    public void SyncSettings()
    {
        if (mirrorTarget != null)
        {
            var mirroredComponent = mirrorTarget.GetComponent<MirroredObject>();
            if (mirroredComponent != null)
            {
                // Apply settings without causing recursion
                mirroredComponent.mirrorRotation = this.mirrorRotation;
                mirroredComponent.mirrorPosition = this.mirrorPosition;
                mirroredComponent.mirrorAxis = this.mirrorAxis;

                // Force update to apply new settings
                mirroredComponent.UpdateMirror();
            }
        }
    }

    public void UpdateMirror()
    {
        if (mirrorTarget == null) return;

        if (mirrorPosition)
        {
            var mirroredPosition = transform.position;
            switch (mirrorAxis)
            {
                case Axis.X:
                    mirroredPosition.x = -mirroredPosition.x;
                    break;
                case Axis.Y:
                    mirroredPosition.y = -mirroredPosition.y;
                    break;
                case Axis.Z:
                    mirroredPosition.z = -mirroredPosition.z;
                    break;
            }
            mirrorTarget.transform.position = mirroredPosition;
        }

        if (mirrorRotation)
        {
            var originalRotation = transform.rotation;
            var mirroredRotation = originalRotation;

            switch (mirrorAxis)
            {
                case Axis.X:
                    mirroredRotation.z = -originalRotation.z ;
                    break;
                case Axis.Y:
                    mirroredRotation.y = originalRotation.y;
                    break;
                case Axis.Z:
                    mirroredRotation.z = originalRotation.z;
                    break;
            }

            mirroredRotation.w = originalRotation.w;

            mirrorTarget.transform.rotation = mirroredRotation;
        }

        if (mirrorScale)
        {
            var originalScale = transform.localScale;
            var mirroredScale = originalScale;
            mirrorTarget.transform.localScale = mirroredScale;
        }
    }
}
