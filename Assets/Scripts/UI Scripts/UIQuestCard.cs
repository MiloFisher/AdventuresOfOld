using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIQuestCard : MonoBehaviour
{
    [Header("Generic Components")]
    public string cardName;
    public GameObject cardBack;
    public Image displayImage;
    public TMP_Text displayName;
    public TMP_Text displayObjectives;
    public TMP_Text displayRewards;
    public float fadeLength = 0.004f;

    public void SetVisuals(QuestCard quest)
    {
        cardName = quest.cardName;
        displayImage.sprite = quest.image;
        displayName.text = cardName;
        string objectives = "";
        for(int i = 0; i < quest.objectiveNames.Length; i++)
        {
            if (quest.questStep > i)
                objectives += "– Completed –";
            else
            {
                objectives += "• " + quest.objectiveNames[i];
                if (quest.objectiveXPRewards[i] > 0)
                    objectives += " (" + quest.objectiveXPRewards[i] + " XP)";
            }
                
            if (i < quest.objectiveNames.Length - 1)
                objectives += "\n";
        }
        displayObjectives.text = objectives;
        string chaos = quest.rewardChaos != 0 ? quest.rewardChaos + " Chaos" : "none";
        string gold = quest.rewardGold != 0 ? "+" + quest.rewardGold + " Gold" : "none";
        if (chaos == "none")
            displayRewards.text = gold;
        else if (gold == "none")
            displayRewards.text = chaos;
        else
            displayRewards.text = chaos + "   " + gold;
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }
}
