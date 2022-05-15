using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIPlayerDisplay : MonoBehaviour
{
    public Image characterImage;
    public TMP_Text characterName;
    public GameObject healthbar;
    public TMP_Text healthbarText;
    public GameObject selectButton;
    public float fadeLength = 0.004f;

    private bool fading;
    private bool visible;

    public void UpdatePanel(Player p)
    {
        characterImage.sprite = PlayManager.Instance.portaitDictionary[p.Image.Value];
        characterName.text = p.Name.Value + "";
        characterName.color = PlayManager.Instance.GetPlayerColor(p);
        int health = PlayManager.Instance.GetHealth(p);
        int maxHealth = PlayManager.Instance.GetMaxHealth(p);
        healthbarText.text = health + " / " + maxHealth;
        healthbar.transform.localPosition = new Vector3(195f * health / maxHealth - 195f, 0, 0);
    }

    public void ActivateSelectButton(bool active)
    {
        selectButton.SetActive(active);
    }

    public bool IsFading()
    {
        return fading;
    }

    public bool IsVisible()
    {
        return visible;
    }

    public void FadeInDisplay()
    {
        if(!fading && !visible && gameObject.activeInHierarchy)
            StartCoroutine(FadeIn());
    }

    public void FadeOutDisplay()
    {
        if(!fading && visible && gameObject.activeInHierarchy)
            StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        fading = true;
        for(int i = 1; i < Global.animSteps; i++)
        {
            SetAlpha(i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }
        fading = false;
        visible = true;
    }

    IEnumerator FadeOut()
    {
        fading = true;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }
        fading = false;
        visible = false;
    }

    private void SetAlpha(float a)
    {
        Image[] images = GetComponentsInChildren<Image>();
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        foreach (Image img in images)
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        foreach (TMP_Text txt in texts)
            txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, a);
    }
}
