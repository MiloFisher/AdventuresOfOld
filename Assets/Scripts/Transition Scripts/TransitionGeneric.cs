using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EndEffect { START_MOVEMENT_PHASE, START_ENCOUNTER_PHASE, START_END_OF_DAY, START_COMBAT_TURN, START_MONSTER_TURN, START_MINION_TURN };

public class TransitionGeneric : MonoBehaviour
{
    public Image background;
    public Image[] images;
    public TMP_Text[] texts;
    public float fadeLength = 0.004f;
    public float waitTime = 1f;
    public float fadeAlpha = 0.785f;
    public EndEffect endEffect;

    public void OnEnable()
    {
        ResetFade();
        StartCoroutine(FadeSequence());
    }

    public void OnDisable()
    {
        switch(endEffect)
        {
            case EndEffect.START_MOVEMENT_PHASE:
                PlayManager.Instance.MovePhase();
                break;
            case EndEffect.START_ENCOUNTER_PHASE:
                PlayManager.Instance.EncounterPhase();
                break;
            case EndEffect.START_END_OF_DAY:
                PlayManager.Instance.EndOfDay();
                break;
            case EndEffect.START_COMBAT_TURN:
                CombatManager.Instance.TakeTurn();
                break;
            case EndEffect.START_MONSTER_TURN:
                CombatManager.Instance.MonsterTakeTurn();
                CombatManager.Instance.isMonsterTurn = false;
                break;
            case EndEffect.START_MINION_TURN:
                CombatManager.Instance.MinionTakeTurn();
                break;
        }
    }

    IEnumerator FadeSequence()
    {
        if(endEffect == EndEffect.START_ENCOUNTER_PHASE)
            JLAudioManager.Instance.PlayOneShotSound("TransitionTone1");
        else
            JLAudioManager.Instance.PlayOneShotSound("TransitionTone2");
        // Fade in background, images, and texts
        for (int i = 1; i <= Global.animSteps; i++)
        {
            SetAlpha(background, i * fadeAlpha * Global.animRate);
            for (int j = 0; j < images.Length; j++)
                SetAlpha(images[j], i * Global.animRate);
            for (int j = 0; j < texts.Length; j++)
                SetAlpha(texts[j], i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        yield return new WaitForSeconds(waitTime * Global.animSpeed);

        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            SetAlpha(background, i * fadeAlpha * Global.animRate);
            for (int j = 0; j < images.Length; j++)
                SetAlpha(images[j], i * Global.animRate);
            for (int j = 0; j < texts.Length; j++)
                SetAlpha(texts[j], i * Global.animRate);
            yield return new WaitForSeconds(fadeLength * Global.animTimeMod * Global.animSpeed);
        }

        gameObject.SetActive(false);
    }

    private void ResetFade()
    {
        SetAlpha(background, 0);
        for (int j = 0; j < images.Length; j++)
            SetAlpha(images[j], 0);
        for (int j = 0; j < texts.Length; j++)
            SetAlpha(texts[j], 0);
    }

    private void SetAlpha(Image i, float a)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, a);
    }

    private void SetAlpha(TMP_Text t, float a)
    {
        t.color = new Color(t.color.r, t.color.g, t.color.b, a);
    }
}
