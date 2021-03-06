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
    public Color statusEffectDebuffColor;
    public Color statusEffectBuffColor;
    public UIEncounterCard minionCard;
    public GameObject toggleViewButton;

    private List<GameObject> statusEffectList = new List<GameObject>();
    private bool blockSwapping;

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
  
        if (combatant.minion == default)
        {
            toggleViewButton.SetActive(false);
            minionCard.gameObject.SetActive(false);
        }
        else
        {
            toggleViewButton.SetActive(!blockSwapping);
            toggleViewButton.GetComponentInChildren<TMP_Text>().text = minionCard.gameObject.activeInHierarchy ? "Show Player" : "Show Minion";
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (combatant == null)
            return;
        if(minionCard.gameObject.activeInHierarchy)
        {
            healthBarText.text = combatant.minion.GetHealth() + " / " + combatant.minion.GetMaxHealth();
            healthBar.transform.localPosition = new Vector3(1454f * combatant.minion.GetHealth() / combatant.minion.GetMaxHealth() - 1454f, 0, 0);
        }
        else
        {
            healthBarText.text = combatant.GetHealth() + " / " + combatant.GetMaxHealth();
            healthBar.transform.localPosition = new Vector3(1454f * combatant.GetHealth() / combatant.GetMaxHealth() - 1454f, 0, 0);
        }
    }

    public void ClickCard(int id)
    {

    }

    public void DrawStatusEffects(List<Effect> effects, bool isMinion = false)
    {
        if (isMinion != minionCard.gameObject.activeInHierarchy)
            return;
        // First previous effects
        for (int i = 0; i < statusEffectList.Count; i++)
            Destroy(statusEffectList[i]);
        statusEffectList.Clear();

        // Then create new effects
        for (int i = 0; i < effects.Count; i++)
        {
            GameObject g = Instantiate(statusEffectPrefab, statusEffectsContainer);
            g.GetComponent<Image>().color = IsDebuff(effects[i]) ? statusEffectDebuffColor : statusEffectBuffColor;
            g.transform.GetChild(0).GetComponent<Image>().sprite = GetEffectSprite(effects[i]);
            g.GetComponentInChildren<StatusEffectTooltip>().SetupDisplay(effects[i]);
            g.transform.localPosition = new Vector3(0, -50 * i, 0);
            statusEffectList.Add(g);
        }
            
    }

    public bool IsDebuff(Effect e)
    {
        return e.name switch
        {
            "Bleeding" => true,
            "Poisoned" => true,
            "Weakened" => true,
            "Power Up" => false,
            "Attack Up" => false,
            "Armor Up" => false,
            "Burning" => true,
            "Dazed" => true,
            "Eaten" => true,
            "Enwebbed" => true,
            "Plagued" => true,
            "Power Down" => true,
            "Power Fantasy" => false,
            "Vanish" => false,
            "Flaming Shot" => false,
            "Bonus Power" => false,
            "Cursed" => true,
            _ => true
        };
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
            "Burning" => statusEffectIcons[6],
            "Dazed" => statusEffectIcons[7],
            "Eaten" => statusEffectIcons[8],
            "Enwebbed" => statusEffectIcons[9],
            "Plagued" => statusEffectIcons[10],
            "Power Down" => statusEffectIcons[11],
            "Power Fantasy" => statusEffectIcons[12],
            "Vanish" => statusEffectIcons[13],
            "Flaming Shot" => statusEffectIcons[14],
            "Bonus Power" => statusEffectIcons[15],
            "Cursed" => statusEffectIcons[16],
            _ => null
        };
    }

    public void ActivateTurnMarker(bool active)
    {
        if (minionCard.gameObject.activeInHierarchy)
            active = false;
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
        if (minionCard.gameObject.activeInHierarchy)
            active = false;
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

    public void ToggleView()
    {
        if (blockSwapping)
            return;
        if(minionCard.gameObject.activeInHierarchy)
        {
            minionCard.gameObject.SetActive(false);
            DrawStatusEffects(combatant.statusEffects, false);
        }
        else
        {
            minionCard.gameObject.SetActive(true);
            DrawStatusEffects(combatant.minion.statusEffects, true);
        }
    }

    public void SetMinionCardActive(bool active)
    {
        minionCard.gameObject.SetActive(active);
        if(active)
            DrawStatusEffects(combatant.minion.statusEffects, true);
        else
            DrawStatusEffects(combatant.statusEffects, false);
    }

    public void BlockSwapping(bool active)
    {
        blockSwapping = active;
    }

    public void ResetMinionVisuals()
    {
        toggleViewButton.SetActive(false);
        minionCard.gameObject.SetActive(false);
    }
}
