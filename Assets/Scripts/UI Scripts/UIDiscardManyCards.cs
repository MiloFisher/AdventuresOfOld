using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDiscardManyCards : MonoBehaviour
{
    public GameObject banner;
    public GameObject finishedButton;
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;
    private int hiddenDiscarded;
    public int discarded;

    private string[] gear;

    private void OnEnable()
    {
        hiddenDiscarded = 0;
        discarded = -1;
        gear = new string[9];
        InventoryManager.Instance.GetGear().CopyTo(gear, 0);
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        discarded = hiddenDiscarded;
    }

    public void CardDiscarded()
    {
        hiddenDiscarded++;
    }

    public void DiscardComplete()
    {
        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateOpening()
    {
        // First setup banner
        banner.GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth, constHeight);
        banner.transform.localScale = new Vector3(startScale, startScale, 1);

        // Then grow the object
        float dif = endScale - startScale;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            banner.transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // Next open the scroll
        dif = endWidth - startWidth;
        for (int i = 1; i <= Global.animSteps; i++)
        {
            banner.GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally force the inventory to open
        InventoryManager.Instance.forcedMaximize = true;
        yield return new WaitUntil(() => !InventoryManager.Instance.inAnimation);
        if (!InventoryManager.Instance.maximized)
            InventoryManager.Instance.MaximizeInventory();
        finishedButton.SetActive(true);
    }

    IEnumerator AnimateClosing()
    {
        finishedButton.SetActive(false);

        // First minimize inventory
        InventoryManager.Instance.forcedMaximize = false;
        InventoryManager.Instance.MinimizeInventory();

        JLAudioManager.Instance.PlayOneShotSound("ScrollOpen");
        // Then close the scroll
        float dif = endWidth - startWidth;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            banner.GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth + dif * i * Global.animRate, constHeight);
            yield return new WaitForSeconds(openingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Then shrink the object
        dif = endScale - startScale;
        for (int i = Global.animSteps - 1; i >= 0; i--)
        {
            banner.transform.localScale = new Vector3(startScale + dif * i * Global.animRate, startScale + dif * i * Global.animRate, 1);
            yield return new WaitForSeconds(growingLength * Global.animTimeMod * Global.animSpeed);
        }

        // Finally deactivate
        gameObject.SetActive(false);
    }
}
