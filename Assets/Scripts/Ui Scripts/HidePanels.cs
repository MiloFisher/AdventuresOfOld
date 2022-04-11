using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidePanels : MonoBehaviour
{
    public GameObject TeamSidebar;
    public GameObject SkillsPanel;

    // Start is called before the first frame update
    void Start()
    {
        TeamSidebar = GameObject.Find("Team Sidebar");
        SkillsPanel = GameObject.Find("Skills Panel");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            
            if (TeamSidebar.activeSelf == true)
            {
                TeamSidebar.SetActive(false);
            }
            else
            {
                TeamSidebar.SetActive(true);
            }
            
        }

        if (Input.GetKeyDown("s"))
        {

            if (SkillsPanel.activeSelf == true)
            {
                SkillsPanel.SetActive(false);
            }
            else
            {
                SkillsPanel.SetActive(true);
            }
        }
    }
}
