using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChildActive : MonoBehaviour
{
    [SerializeField]
    bool childActiveStart;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(childActiveStart);
    }
}
