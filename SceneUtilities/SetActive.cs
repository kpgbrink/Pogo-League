using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    [SerializeField]
    bool startActive;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(startActive);
    }
}
