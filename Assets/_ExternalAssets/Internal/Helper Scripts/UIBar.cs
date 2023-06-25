using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [Header("General stats")]
    public float maxAmount;
    public float minAmount;
    [SerializeField] private Image imageFill;
    [SerializeField] CanvasGroup uiBarCanvasGroup;
    
    public void SetValue(float value, float maxValue)
    {
        float calc_value = value / maxValue;
        float outputValue = calc_value * (maxAmount - minAmount) + minAmount;
        imageFill.fillAmount = Mathf.Clamp(outputValue, minAmount, maxAmount);
    }

    public void SetMaxValue(float maxValue)
    {
        float outputMaxValue = maxValue * (maxAmount - minAmount) + minAmount;
        imageFill.fillAmount = Mathf.Clamp(outputMaxValue,minAmount,maxAmount);
    }
    
    public void ToggleCanvasGroup(bool toggle)
    {
        float endValue = toggle ? 1f : 0f;
        StartCoroutine(CanvasAlphaLerp(endValue, 0.15f));
    }
    
    IEnumerator CanvasAlphaLerp(float end, float seconds)
    {
        float timeElapsed = 0;
        float startValue = uiBarCanvasGroup.alpha;
        while (timeElapsed < seconds)
        {
            uiBarCanvasGroup.alpha = Mathf.Lerp(startValue, end, timeElapsed / seconds);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        uiBarCanvasGroup.alpha = end;
    }
}
