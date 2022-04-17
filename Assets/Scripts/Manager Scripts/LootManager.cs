using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : Singleton<LootManager>
{
    public GameObject travelCard;
    public float startX = -1300;
    public float startY = -545.8f;
    public float endX = 0;
    public float endY = 0;
    public float gap = 150;
    public float travelLength = 0.004f;

    public void DrawCard(int amount)
    {
        if(amount > 0)
            StartCoroutine(AnimateCardDraw(0, amount));
    }

    IEnumerator AnimateCardDraw(int current, int amount)
    {
        travelCard.SetActive(true);
        travelCard.GetComponent<UILootCard>().ActivateCardButton(false);
        travelCard.GetComponent<UILootCard>().ActivateCardBack(true);

        float distX = endX - startX - gap * (2 * current + 1 - amount);
        float x = distX * 0.01f - 1;
        float distY = endY - startY;
        float y = Mathf.Log10(1 + x);
        float scale;

        for (int i = 1; i <= 100; i++)
        {
            // Move Position
            travelCard.transform.localPosition = new Vector3(startX + distX * i * 0.01f, startY + Mathf.Log10(1 + x * i * 0.01f) / y * distY, 0);
            // Rotate X, Y, Z
            travelCard.transform.localRotation = Quaternion.Euler(new Vector3(i <= 50 ? 22.5f * i * 0.02f - 45 : 22.5f * (50-(i-50)) * 0.02f, i <= 50 ? -90 * i * 0.02f : 90 * (50 - (i - 50)) * 0.02f, i <= 50 ? 17.6f * (50-i) * 0.02f + 17.6f : -17.6f * (100-i) * 0.02f)); //45 * i * 0.01f - 45, -180 * i * 0.01f, 35.2f * (100-i) * 0.01f
            // Scale up
            scale = 0.25f * i * 0.01f + 0.75f;
            travelCard.transform.localScale = new Vector3(scale, scale, 1);
            // Flip card face halfway through
            if (i == 50)
                travelCard.GetComponent<UILootCard>().ActivateCardBack(false);
            yield return new WaitForSeconds(travelLength);
        }

        //travelCard.SetActive(false);

        if (current < amount - 1)
            StartCoroutine(AnimateCardDraw(current + 1, amount));
    }
}
