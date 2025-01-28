using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{

    public Slider slider;
    public void setMinValue()
    {
        slider.value = 0;
        slider.minValue = 0;
    }
    public void setMaxValue(int value)
    {
        slider.maxValue = value;
    }
    public void setFillValue(int value)
    {
        slider.value = value;
    }

    public void SetFillValueSmooth(int targetValue, float duration)
    {
        // Stop any existing smooth transitions
        StopAllCoroutines();

        // Start the new smooth transition
        StartCoroutine(SmoothTransition(targetValue, duration));
    }

    private IEnumerator SmoothTransition(int targetValue, float duration)
    {
        float startValue = slider.value;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }

        // Ensure the final value is set precisely
        slider.value = targetValue;
    }

}
