using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIPlayerCard : MonoBehaviour
{
    public Player player;
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

    public void SetVisuals(Player p)
    {
        player = p;
        playerImage.sprite = PlayManager.Instance.portaitDictionary[player.Image.Value];
        playerFaceColor.color = PlayManager.Instance.GetPlayerColor(player);
        playerName.text = player.Name.Value + "";
        playerArmor.text = PlayManager.Instance.GetArmor(player) + "";
        playerAttack.text = PlayManager.Instance.GetAttack(player) + "";
        playerSpeed.text = PlayManager.Instance.GetSpeed(player) + "";
        playerPhysicalPower.text = PlayManager.Instance.GetPhysicalPower(player) + "";
        playerMagicalPower.text = PlayManager.Instance.GetMagicalPower(player) + "";
        playerDescription.text = player.Trait.Value + " " + player.Race.Value + " " + player.Class.Value;
    }

    public void ActivateCardBack(bool active)
    {
        cardBack.SetActive(active);
    }

    public void ActivateCardButton(bool active)
    {
        cardButton.SetActive(active);
    }
}