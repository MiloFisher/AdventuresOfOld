using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdventuresOfOldMultiplayer;

public class UIAttackRoll : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    public Sprite[] diceFaces;
    public TMP_Text playerPower;
    public TMP_Text monsterPower;
    public Image rollDisplay1;
    public Image rollDisplay2;
    public GameObject rollButton;
    public TMP_Text title;
    public float rollLength = 0.01f;
    public float rollDisplayTime = 1f;
    public float waitTime = 0.5f;
    public GameObject successText;
    public GameObject failureText;
    public GameObject tieText;
    public int success;
    public bool crit;

    private int hiddenSuccess;
    private RectTransform rt;
    private bool opened;
    private int roll1 = 0;
    private int roll2 = 0;
    private bool rolling;
    private bool focusUsed;

    private int playerPowerValue;
    private int monsterPowerValue;

    private Combatant attackingPlayer;

    public void MakeAttackRoll(Combatant c, int _monsterPower, string attackName)
    {
        crit = false;
        success = 0;
        attackingPlayer = c;
        if (PlayManager.Instance.IsPhysicalBased(attackingPlayer.player))
            playerPowerValue = attackingPlayer.GetPhysicalPower();
        else
            playerPowerValue = attackingPlayer.GetMagicalPower();
        monsterPowerValue = _monsterPower;
        playerPower.text = playerPowerValue + "";
        monsterPower.text = monsterPowerValue + "";
        title.text = "– " + attackName + " –";
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        ResetSize();
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        success = hiddenSuccess;
    }

    private void Update()
    {
        if(opened)
        {
            if (PlayManager.Instance.IsPhysicalBased(attackingPlayer.player))
                playerPowerValue = attackingPlayer.GetPhysicalPower();
            else
                playerPowerValue = attackingPlayer.GetMagicalPower();
            playerPower.text = playerPowerValue + "";
            if (CombatManager.Instance.CombatOverCheck() > -1 && !rolling)
            {
                rolling = true;
                hiddenSuccess = 99;
                StartCoroutine(AnimateClosing());
            }
        }
    }

    IEnumerator AnimateOpening()
    {
        // First grow the object
        float dif = endScale - startScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Next open the scroll
        dif = endWidth - startWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally set opened to true
        opened = true;
    }

    public void ResetSize()
    {
        focusUsed = false;
        rolling = false;
        hiddenSuccess = 0;
        opened = false;
        rt = GetComponent<RectTransform>();
        transform.localScale = new Vector3(startScale, startScale, 1);
        rt.sizeDelta = new Vector2(startWidth, constHeight);
        rollButton.SetActive(true);
        successText.SetActive(false);
        failureText.SetActive(false);
    }

    public void RollDice()
    {
        // Return if the scroll hasn't opened yet
        if (!opened && !rolling)
            return;

        rollButton.SetActive(false);
        roll1 = Random.Range(1, 7);
        roll2 = Random.Range(1, 7);
        StartCoroutine(AnimateDiceRoll());
    }

    IEnumerator AnimateDiceRoll()
    {
        rolling = true;

        // Flash through random dice faces
        int rollTimes = Random.Range(80, 121);
        for (int i = 0; i < rollTimes; i++)
        {
            rollDisplay1.sprite = diceFaces[Random.Range(0, 6)];
            rollDisplay2.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animSpeed);
        }

        // End dice 1 first
        rollDisplay1.sprite = diceFaces[roll1 - 1];

        // Keep rolling dice 2
        rollTimes = Random.Range(20, 60);
        for (int i = 0; i < rollTimes; i++)
        {
            rollDisplay2.sprite = diceFaces[Random.Range(0, 6)];
            yield return new WaitForSeconds(rollLength * Global.animSpeed);
        }

        // End dice 2
        rollDisplay2.sprite = diceFaces[roll2 - 1];

        int playerCritValue = PlayManager.Instance.GetCrit(PlayManager.Instance.localPlayer);

        // Display success or failure
        if(roll1 + roll2 >= playerCritValue)
        {
            crit = true;
            successText.SetActive(true);
            hiddenSuccess = 1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else if (roll1 == 1 && roll2 == 1)
        {
            failureText.SetActive(true);
            hiddenSuccess = -1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else if (roll1 + roll2 + playerPowerValue > monsterPowerValue)
        {
            successText.SetActive(true);
            hiddenSuccess = 1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else if (roll1 + roll2 + playerPowerValue < monsterPowerValue)
        {
            if(AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Focus")) && (roll1 == 1 || roll2 == 1) && !focusUsed)
            {
                focusUsed = true;
                PlayManager.Instance.SendNotification(6, "Your (1) is being re-rolled!", () => {
                    StartCoroutine(Reroll1(roll1 == 1));
                });
            }
            else
            {
                failureText.SetActive(true);
                hiddenSuccess = -1;

                yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

                // Start closing scroll
                StartCoroutine(AnimateClosing());
            }
        }
        else
        {
            if (AbilityManager.Instance.HasAbilityUnlocked(AbilityManager.Instance.GetSkill("Focus")) && (roll1 == 1 || roll2 == 1) && !focusUsed)
            {
                focusUsed = true;
                PlayManager.Instance.SendNotification(6, "Your (1) is being re-rolled!", () => {
                    StartCoroutine(Reroll1(roll1 == 1));
                });
            }
            else
            {
                tieText.SetActive(true);

                yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

                rolling = false;
                tieText.SetActive(false);
                rollButton.SetActive(true);
            }
        }
    }

    IEnumerator Reroll1(bool targetRoll1)
    {
        int rollTimes = Random.Range(80, 121);

        if (targetRoll1)
        {
            roll1 = Random.Range(1, 7);

            // Flash through random dice faces
            for (int i = 0; i < rollTimes; i++)
            {
                rollDisplay1.sprite = diceFaces[Random.Range(0, 6)];
                yield return new WaitForSeconds(rollLength * Global.animSpeed);
            }

            // End dice 1
            rollDisplay1.sprite = diceFaces[roll1 - 1];
        }
        else
        {
            roll2 = Random.Range(1, 7);

            // Flash through random dice faces
            for (int i = 0; i < rollTimes; i++)
            {
                rollDisplay2.sprite = diceFaces[Random.Range(0, 6)];
                yield return new WaitForSeconds(rollLength * Global.animSpeed);
            }

            // End dice 2
            rollDisplay2.sprite = diceFaces[roll2 - 1];
        }

        int playerCritValue = PlayManager.Instance.GetCrit(PlayManager.Instance.localPlayer);

        // Display success or failure
        if (roll1 + roll2 >= playerCritValue)
        {
            crit = true;
            successText.SetActive(true);
            hiddenSuccess = 1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else if (roll1 == 1 && roll2 == 1)
        {
            failureText.SetActive(true);
            hiddenSuccess = -1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else if (roll1 + roll2 + playerPowerValue > monsterPowerValue)
        {
            successText.SetActive(true);
            hiddenSuccess = 1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else if (roll1 + roll2 + playerPowerValue < monsterPowerValue)
        {
            failureText.SetActive(true);
            hiddenSuccess = -1;

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            // Start closing scroll
            StartCoroutine(AnimateClosing());
        }
        else
        {
            tieText.SetActive(true);

            yield return new WaitForSeconds(rollDisplayTime * Global.animSpeed);

            rolling = false;
            tieText.SetActive(false);
            rollButton.SetActive(true);
        }
    }

    IEnumerator AnimateClosing()
    {
        opened = false;

        // First close the scroll
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            rt.sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Next shrink the object
        dif = endScale - startScale;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        yield return new WaitForSeconds(waitTime * Global.animSpeed);

        gameObject.SetActive(false);
        PlayManager.Instance.localPlayer.SetValue("HasYetToAttack", false);
    }
}
