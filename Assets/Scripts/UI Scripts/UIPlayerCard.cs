using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIPlayerCard : MonoBehaviour
{
    public Combatant combatant;
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
    public GameObject playerHealed;
    public GameObject damageNumber;
    public GameObject healNumber;
    public Transform statusEffectsContainer;
    public GameObject statusEffectPrefab;
    public Sprite[] statusEffectIcons;
    public float damageNumberFadeLength = 0.004f;

    private List<GameObject> statusEffectList = new List<GameObject>();

    public void SetVisuals(Combatant c)
    {
        combatant = c;
        playerImage.sprite = PlayManager.Instance.portaitDictionary[combatant.player.Image.Value];
        playerFaceColor.color = PlayManager.Instance.GetPlayerColor(combatant.player);
        cardBack.transform.GetChild(0).GetComponent<Image>().color = PlayManager.Instance.GetPlayerColor(combatant.player);
        playerName.text = combatant.GetName();
        playerArmor.text = combatant.GetArmor().ToString();
        playerAttack.text = combatant.GetAttack().ToString();
        playerSpeed.text = combatant.GetSpeed().ToString();
        playerPhysicalPower.text = combatant.GetPhysicalPower().ToString();
        playerMagicalPower.text = combatant.GetMagicalPower().ToString();
        playerDescription.text = combatant.player.Trait.Value + " " + combatant.player.Race.Value + " " + combatant.player.Class.Value;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (combatant == null)
            return;
        healthBarText.text = combatant.GetHealth() + " / " + combatant.GetMaxHealth();
        healthBar.transform.localPosition = new Vector3(1454f * combatant.GetHealth() / combatant.GetMaxHealth() - 1454f, 0, 0);
    }

    public void ClickCard(int id)
    {

    }

    public void DrawStatusEffects(List<Effect> effects)
    {
        // First previous effects
        for (int i = 0; i < statusEffectList.Count; i++)
            Destroy(statusEffectList[i]);
        statusEffectList.Clear();

        // Then create new effects
        for (int i = 0; i < effects.Count; i++)
        {
            GameObject g = Instantiate(statusEffectPrefab, statusEffectsContainer);
            g.GetComponent<Image>().sprite = GetEffectSprite(effects[i]);
            g.transform.localPosition = new Vector3(0, -50 * i, 0);
            statusEffectList.Add(g);
        }
            
    }

    public Sprite GetEffectSprite(Effect e)
    {
        return e.name switch
        {
            "Bleeding" => statusEffectIcons[0],
            "Poisoned" => statusEffectIcons[1],
            "Weakened" => statusEffectIcons[2],
            "Power Up" => statusEffectIcons[3],
            "Attack Up" => statusEffectIcons[4],
            "Armor Up" => statusEffectIcons[5],
            _ => null
        };
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

    public void ActivateHealed(bool active)
    {
        playerHealed.SetActive(active);
    }

    public void DisplayDamageNumber(int amount)
    {
        if (amount < 0)
            amount = 0;
        Vector3 startPosition = healthBarBack.transform.localPosition + new Vector3(0.2f * (healthBar.GetComponent<RectTransform>().sizeDelta.x / 2 + healthBar.transform.localPosition.x), 0, 0);
        damageNumber.GetComponent<TMP_Text>().text = "-" + amount;
        damageNumber.SetActive(true);
        SetAlpha(damageNumber.GetComponent<TMP_Text>(), 1);
        StartCoroutine(AnimateNumber(damageNumber, startPosition));
    }

    IEnumerator AnimateNumber(GameObject number, Vector3 startPosition)
    {
        number.transform.localPosition = startPosition;

        // Fade in and float number
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(number.GetComponent<TMP_Text>(), 1 - i * Global.animRate);
            number.transform.localPosition = startPosition + new Vector3(i * Global.animRate * 50, i * Global.animRate * 100, 0);
            yield return new WaitForSeconds(damageNumberFadeLength * Global.animTimeMod * Global.animSpeed);
        }

        number.SetActive(false);
    }

    public void DisplayHealNumber(int amount)
    {
        if (amount < 0)
            amount = 0;
        Vector3 startPosition = healthBarBack.transform.localPosition + new Vector3(0.2f * (healthBar.GetComponent<RectTransform>().sizeDelta.x / 2 + healthBar.transform.localPosition.x), 0, 0);
        healNumber.GetComponent<TMP_Text>().text = "+" + amount;
        healNumber.SetActive(true);
        SetAlpha(healNumber.GetComponent<TMP_Text>(), 1);
        StartCoroutine(AnimateNumber(healNumber, startPosition));
    }

    private void SetAlpha(TMP_Text t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
