using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    List<Tab> tabButtons;
    
    [SerializeField]
    Sprite tabIdle;
    [SerializeField]
    Sprite tabHover;
    [SerializeField]
    Sprite tabActive;
    [SerializeField]
    Tab selectedTab;

    private void Start()
    {
        // Set 
        foreach (Transform child in transform)
        {
            //Debug.Log("Addding a child");
            var tabButton = child.GetComponent<Tab>();
            if (tabButton == null)
                Debug.LogError("Must have a TabButton if child of TabGroup");
            Subscribe(tabButton);
        }
        ResetTabs();
        ResetTabPages();
    }

    public void Subscribe(Tab button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<Tab>();
        }
        button.TabGroup = this;
        tabButtons.Add(button);
    }

    public void OnTabEnter(Tab button)
    {
        ResetTabs();
        if (selectedTab == null && button == selectedTab) return;
        button.background.sprite = tabHover;
    }

    public void OnTabExit(Tab button)
    {
        ResetTabs();
    }

    public void OnTabSelected(Tab button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        
        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        button.background.sprite = tabActive;
        ResetTabPages();
    }

    void ResetTabPages()
    {
        // Set only the tab page of the selected button to active
        foreach (var b in tabButtons)
        {
            b.tabPage.SetActive(b == selectedTab);
        }
    }

    void ResetTabs()
    {
        foreach (var button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
            {
                button.background.sprite = tabActive;
                continue;
            }
            //Debug.Log(tabIdle);
            //Debug.Log(button.background.sprite);
            button.background.sprite = tabIdle;
        }
    }

    int? GetSelectedTabIndex()
    {
        var i = 0;
        foreach(var button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) return i;
            i++;
        }
        return null;
    }

    public void LeftTabPressed()
    {
        Debug.Log("Left tab pressed");
        var selectedIndex = GetSelectedTabIndex();
        if (selectedIndex == null) return;
        var index = Math.Max((int)selectedIndex - 1, 0);
        OnTabSelected(tabButtons[index]);
    }

    public void RightTabPressed()
    {
        Debug.Log("Right tab pressed");
        var selectedIndex = GetSelectedTabIndex();
        if (selectedIndex == null) return;
        var index = Math.Min((int)selectedIndex + 1, tabButtons.Count - 1);
        OnTabSelected(tabButtons[index]);
    }
}
