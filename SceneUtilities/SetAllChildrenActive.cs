using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAllChildrenActive : MonoBehaviour
{
    [SerializeField]
    bool childrenActiveStart;

    // Update is called once per frame
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(childrenActiveStart);
        }
    }
}
