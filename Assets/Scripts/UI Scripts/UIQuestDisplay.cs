using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIQuestDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject questCardPrefab;
    public float scale = 2;
    public float gap = 150;
    public List<GameObject> questCards = new List<GameObject>();
    private bool open;

    public void SetupQuests()
    {
        for(int i = 0; i < questCards.Count; i++)
            Destroy(questCards[i]);
        questCards.Clear();
        foreach (QuestCard q in PlayManager.Instance.quests)
        {
            GameObject g = Instantiate(questCardPrefab, transform.parent);
            g.GetComponent<UIQuestCard>().SetVisuals(q);
            g.transform.localScale = new Vector3(scale, scale, 1);
            questCards.Add(g);
        }
        HideQuestCards();
        if(open)
            DisplayQuestCards();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        open = true;
        DisplayQuestCards();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        open = false;
        HideQuestCards();
    }

    private void DisplayQuestCards()
    {
        for(int i = 0; i < questCards.Count; i++)
        {
            questCards[i].SetActive(true);
            questCards[i].transform.localPosition = new Vector3(scale * gap * (2 * i + 1 - questCards.Count), 0, 0);
        }
    }

    private void HideQuestCards()
    {
        foreach (GameObject g in questCards)
            g.SetActive(false);
    }
}