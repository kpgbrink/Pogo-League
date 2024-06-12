using UnityEngine;

public class AddRemoveDynamicCamera : MonoBehaviour
{
    [SerializeField]
    DynamicCameraController dynamicCameraController;

    [SerializeField]
    bool StartAddedToCameraController = false;

    private void Start()
    {
        if (StartAddedToCameraController)
        {
            this.AddToDynamicCameraController();
        }
    }
    
    public void AddToDynamicCameraController()
    {
        // Add this to the list of targets in the DynamicCameraController
        dynamicCameraController.targets.Add(this.transform);
    }

    public void RemoveFromDynamicCameraController()
    {
        // Remove this from the list of targets in the DynamicCameraController
        dynamicCameraController.targets.Remove(this.transform);
    }
}
