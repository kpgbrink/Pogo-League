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
        try
        {
            // Remove this from the list of targets in the DynamicCameraController
            dynamicCameraController.targets.Remove(this.transform);
        }
        catch (System.Exception e)
        {
            Debug.Log($"Error removing from dynamic camera controller: {e.Message}");
        }
    }

    void OnDestroy()
    {
        RemoveFromDynamicCameraController();
    }
}
