using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCameraController : MonoBehaviour
{
    [SerializeField]
    public List<Transform> targets;
    public float verticalOffset = 10f; // Vertical offset from the center point
    public float smoothTime = 0.5f;
    public float minZoom = 10f;
    public float maxZoom = 400f;
    public float zoomLimiter = 50f;
    private Vector3 velocity;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        // Ensure the camera always faces down the positive Z-axis
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        // The camera only moves in the X and Y axes
        Vector3 newPosition = new Vector3(centerPoint.x, centerPoint.y + verticalOffset, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return Mathf.Max(bounds.size.x, bounds.size.y); // Get the greater of the width or height
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    void OnDrawGizmos()
    {
        if (targets.Count == 0)
            return;

        // Draw a sphere at the center point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetCenterPoint(), 0.5f);

        // Draw the bounds
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
