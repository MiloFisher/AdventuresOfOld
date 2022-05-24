using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICombatCharacterPanel : MonoBehaviour
{
    public Image nameplate;
    public Image characterImage;
    public Image monsterImage;
    public TMP_Text characterName;
    public GameObject healthbar;
    public TMP_Text healthbarText;
    public Color friendlyColor;
    public Color enemyColor;

    public void UpdatePanel(Combatant c)
    {
        if (c.combatantType == CombatantType.PLAYER)
        {
            characterImage.gameObject.SetActive(true);
            monsterImage.gameObject.SetActive(false);
            characterImage.sprite = PlayManager.Instance.portaitDictionary[c.player.Image.Value];

            nameplate.color = friendlyColor;

            characterName.text = c.player.Name.Value + "";
        }
        else if (c.combatantType == CombatantType.MONSTER)
        {
            characterImage.gameObject.SetActive(false);
            monsterImage.gameObject.SetActive(true);
            monsterImage.sprite = c.monster.image;

            nameplate.color = enemyColor;

            characterName.text = c.monster.cardName;
        }
        else
        {
            characterImage.gameObject.SetActive(false);
            monsterImage.gameObject.SetActive(true);
            monsterImage.sprite = c.monster.image;

            nameplate.color = friendlyColor;

            characterName.text = c.monster.cardName;
        }
        int health = c.GetHealth();
        int maxHealth = c.GetMaxHealth();
        healthbarText.text = health + " / " + maxHealth;
        if(maxHealth > 0)
            healthbar.transform.localPosition = new Vector3(195f * health / maxHealth - 195f, 0, 0);
    }
}