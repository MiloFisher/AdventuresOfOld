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
        else
        {
            characterImage.gameObject.SetActive(false);
            monsterImage.gameObject.SetActive(true);
            monsterImage.sprite = c.monster.image;

            nameplate.color = enemyColor;

            characterName.text = c.monster.cardName;
        }
        healthbarText.text = c.GetHealth() + " / " + c.GetMaxHealth();
        healthbar.transform.localPosition = new Vector3(195f * c.GetHealth() / c.GetMaxHealth() - 195f, 0, 0);
    }
}