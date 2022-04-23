using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIForcedDiscard : MonoBehaviour
{
    public float startWidth;
    public float endWidth;
    public float constHeight;
    public float startScale;
    public float endScale;
    public float growingLength = 0.004f;
    public float openingLength = 0.004f;

    public bool completed;

    private string[] gear;

    private void OnEnable()
    {
        gear = new string[9];
        InventoryManager.Instance.GetGear().CopyTo(gear, 0);
        completed = false;
        StartCoroutine(AnimateOpening());
    }

    private void OnDisable()
    {
        completed = true;
    }

    public void DiscardComplete()
    {
        StartCoroutine(AnimateClosing());
    }

    IEnumerator AnimateOpening()
    {
        // First setup banner
        GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth, constHeight);
        transform.localScale = new Vector3(startScale, startScale, 1);

        // Then grow the object
        float dif = endScale - startScale;
        for (int i = 1; i <= 100; i++)
        {
            transform.localScale = new Vector3(startScale + dif * i * 0.01f, startScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(growingLength);
        }

        // Next open the scroll
        dif = endWidth - startWidth;
        for (int i = 1; i <= 100; i++)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth + dif * i * 0.01f, constHeight);
            yield return new WaitForSeconds(openingLength);
        }

        // Finally force the inventory to open
        InventoryManager.Instance.forcedMaximize = true;
        yield return new WaitUntil(() => !InventoryManager.Instance.inAnimation);
        if (!InventoryManager.Instance.maximized)
            InventoryManager.Instance.MaximizeInventory();
    }

    IEnumerator AnimateClosing()
    {
        // First minimize inventory
        InventoryManager.Instance.forcedMaximize = false;
        InventoryManager.Instance.MinimizeInventory();

        // Then close the scroll
        float dif = endWidth - startWidth;
        for (int i = 99; i >= 0; i--)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(startWidth + dif * i * 0.01f, constHeight);
            yield return new WaitForSeconds(openingLength);
        }

        // Then shrink the object
        dif = endScale - startScale;
        for (int i = 99; i >= 0; i--)
        {
            transform.localScale = new Vector3(startScale + dif * i * 0.01f, startScale + dif * i * 0.01f, 1);
            yield return new WaitForSeconds(growingLength);
        }

        // Finally deactivate banner
        gameObject.SetActive(false);
    }
}