using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Sets what is started on selecting
/// </summary>
public class SelectionHandling : MonoBehaviour
{
    public GameObject selected;

    // Update is called once per frame
    void Update()
    {
        SelectedHandling();
    }

    private void OnEnable()
    {
        StartCoroutine(SetToSelectedNextFrame());
    }

    void SelectedHandling()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected != null)
        {
            if (currentSelected.transform.IsChildOf(transform))
            {
                selected = currentSelected;
            }
            return;
        }
        SetToSelected();
    }

    void SetToSelected()
    {
        StartCoroutine(SetToSelectedNextFrame());
    }

    IEnumerator SetToSelectedNextFrame()
    {
        var current = EventSystem.current;
        yield return null;
        current.SetSelectedGameObject(null);
        current.SetSelectedGameObject(selected);
    }
}
