using UnityEngine;

public class CameraUI : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    Camera cameraUI;

    // Start is called before the first frame update
    void Start()
    {
        canvas.planeDistance = cameraUI.nearClipPlane + .1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
