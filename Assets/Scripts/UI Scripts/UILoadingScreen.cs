using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILoadingScreen : MonoBehaviour
{
    public TMP_Text loadingText;
    public float pulseTime;

    private int step;

    private void OnEnable()
    {
        step = 0;
        StartCoroutine(AnimateLoading());
    }

    IEnumerator AnimateLoading()
    {
        for(; ; )
        {
            if (step == 0)
                loadingText.text = "Loading";
            else if (step == 1)
                loadingText.text = "Loading.";
            else if (step == 2)
                loadingText.text = "Loading..";
            else if (step == 3)
                loadingText.text = "Loading...";

            step++;

            if (step > 3)
                step = 0;
            yield return new WaitForSeconds(pulseTime);
        }
    }
}
