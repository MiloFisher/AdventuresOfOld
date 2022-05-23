using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICharacterPanel : MonoBehaviour
{
    public int id;
    public Image characterImage;
    public TMP_Text characterName;
    public GameObject healthbar;
    public TMP_Text healthbarText;

    public void UpdateHealthbar(int health, int maxHealth)
    {
        healthbarText.text = health + " / " + maxHealth;
        healthbar.transform.localPosition = new Vector3(195f*health/maxHealth - 195f, 0, 0);
    }

    public void UpdateCharacterImage(Sprite image)
    {
        characterImage.sprite = image;
    }

    public void UpdateCharacterName(string name, string color)
    {
        characterName.text = "<color=" + color + ">" + name + "</color>";
    }

    public void ClickPortrait()
    {
        JLAudioManager.Instance.PlayOneShotSound("ClickSound");
        PlayManager.Instance.SelectPortrait(id);
    }
}
