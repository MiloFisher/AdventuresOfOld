using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JLRulebook : MonoBehaviour
{
    public GameObject title;
    public Sprite[] rulebookPages;
    public GameObject currentPage;
    private int pageID = 0;

    public void titleActive() {
        title.SetActive(!title.activeSelf);
    }

    public void swapPages(int id)
    {
        if (pageID < 0 || (pageID == 0 && id == -1)|| (pageID >= rulebookPages.Length - 1 && id == 1))
            return;
        pageID = pageID + id;
        currentPage.GetComponent<Image>().sprite = rulebookPages[pageID];
    }
}
