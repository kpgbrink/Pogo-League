using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformData
{
    public TransformData(Transform transform)
    {
        Position = transform.position;
        Scale = transform.localScale;
        Rotation = transform.rotation;
    }

    public TransformData(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }

    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Quaternion Rotation { get; set; }
}
