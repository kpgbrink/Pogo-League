using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SetVisibility : MonoBehaviour
{
    [SerializeField]
    bool startVisible;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.GetComponent<Renderer>().enabled = startVisible;
    }
}
