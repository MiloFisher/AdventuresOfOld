using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIPlayerCard : MonoBehaviour
{
    public Player player;
    public GameObject healthBarBack;
    public TMP_Text healthBarText;
    public GameObject healthBar;
    public GameObject cardBack;
    public GameObject cardButton;
    public Image playerImage;
    public Image playerFaceColor;
    public TMP_Text playerName;
    public TMP_Text playerArmor;
    public TMP_Text playerAttack;
    public TMP_Text playerSpeed;
    public TMP_Text playerPhysicalPower;
    public TMP_Text playerMagicalPower;
    public TMP_Text playerDescription;
    public GameObject playerTurnMarker;
    public GameObject targetCrosshair;
    public GameObject playerDamaged;
    public GameObject damageNumber;
    public float damageNumberFadeLength = 0.004f;

    public void SetVisuals(Player p)
    {
        player = p;
        playerImage.sprite = PlayManager.Instance.portaitDictionary[player.Image.Value];
        playerFaceColor.color = PlayManager.Instance.GetPlayerColor(player);
        cardBack.transform.GetChild(0).GetComponent<Image>().color = PlayManager.Instance.GetPlayerColor(player);
        playerName.text = player.Name.Value + "";
        playerArmor.text = PlayManager.Instance.GetArmor(player) + "";
        playerAttack.text = PlayManager.Instance.GetAttack(player) + "";
        playerSpeed.text = PlayManager.Instance.GetSpeed(player) + "";
        playerPhysicalPower.text = PlayManager.Instance.GetPhysicalPower(player) + "";
        playerMagicalPower.text = PlayManager.Instance.GetMagicalPower(player) + "";
        playerDescription.text = player.Trait.Value + " " + player.Race.Value + " " + player.Class.Value;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (player == null)
            return;
        healthBarText.text = PlayManager.Instance.GetHealth(player) + " / " + PlayManager.Instance.GetMaxHealth(player);
        healthBar.transform.localPosition = new Vector3(1454f * PlayManager.Instance.GetHealth(player) / PlayManager.Instance.GetMaxHealth(player) - 1454f, 0, 0);
    }

    public void ClickCard(int id)
    {

    }

    public void ActivateTurnMarker(bool active)
    {
        playerTurnMarker.SetActive(active);
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        cardButton.SetActive(active);
    }

    public void ActivateHealthBar(bool active)
    {
        healthBarBack.SetActive(active);
    }

    public void ActivateCrosshair(bool active)
    {
        targetCrosshair.SetActive(active);
    }

    public void ActivateDamaged(bool active)
    {
        playerDamaged.SetActive(active);
    }

    public void DisplayDamageNumber(int amount)
    {
        if (amount < 0)
            amount = 0;
        Vector3 startPosition = healthBarBack.transform.localPosition + new Vector3(0.2f * (healthBar.GetComponent<RectTransform>().sizeDelta.x / 2 + healthBar.transform.localPosition.x), 0, 0);
        damageNumber.GetComponent<TMP_Text>().text = "-" + amount;
        damageNumber.SetActive(true);
        SetAlpha(damageNumber.GetComponent<TMP_Text>(), 1);
        StartCoroutine(AnimateDamageNumber(startPosition));
    }

    IEnumerator AnimateDamageNumber(Vector3 startPosition)
    {
        damageNumber.transform.localPosition = startPosition;

        // Fade in and float number
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(damageNumber.GetComponent<TMP_Text>(), 1 - i * Global.animRate);
            damageNumber.transform.localPosition = startPosition + new Vector3(i * Global.animRate * 50, i * Global.animRate * 100, 0);
            yield return new WaitForSeconds(damageNumberFadeLength * Global.animTimeMod);
        }

        damageNumber.SetActive(false);
    }

    private void SetAlpha(TMP_Text t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
