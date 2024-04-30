using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public GameObject menu;
    public GameObject tabGroup;
    

    private void Start()
    {
        MenuChildrenInitialize();
    }

    void MenuChildrenInitialize()
    {
        // Each child of menus that implements
        // IMenuInitialize run the initialize method on it
        // And set the proper one to active on start
        foreach (Transform child in menu.transform)
        {
            var menuInitialize = child.GetComponent<IMenuInitialize>();
            menuInitialize?.Initialize();
        }
    }

    public void ToggleLevelSelect()
    {
        menu.SetActive(!menu.activeSelf);
        tabGroup.SetActive(!tabGroup.activeSelf);
    }
}
