using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidePanels : MonoBehaviour
{
    public GameObject TeamSidebar;
    public GameObject SkillsPanel;
    public GameObject CharacterPanel;
    public GameObject HelpPanel;

    // Start is called before the first frame update
    void Start()
    {
        TeamSidebar = GameObject.Find("Team Sidebar");
        SkillsPanel = GameObject.Find("Skills Panel");
        CharacterPanel = GameObject.Find("Character Sheet");
        HelpPanel = GameObject.Find("Help Panel");
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

        if (Input.GetKeyDown("c"))
        {

            if (CharacterPanel.activeSelf == true)
            {
                CharacterPanel.SetActive(false);
            }
            else
            {
                CharacterPanel.SetActive(true);
            }
        }

        if (Input.GetKeyDown("h"))
        {

            if (HelpPanel.activeSelf == true)
            {
                HelpPanel.SetActive(false);
            }
            else
            {
                HelpPanel.SetActive(true);
            }
        }
    }
}
