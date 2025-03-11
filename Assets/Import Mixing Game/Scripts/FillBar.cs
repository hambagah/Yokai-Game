/**
 * FillBar.cs
 * 
 * Summary: Manages a UI slider that represents a fill level.
 * Provides methods to set and animate the slider value with smooth transitions.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [Tooltip("Reference to the UI slider component")]
    public Slider slider;
    
    // Track current animation coroutine
    private Coroutine fillCoroutine = null;

    /**
     * Sets the slider to minimum value (0)
     * Called during initialization
     */
    public void setMinValue()
    {
        slider.value = 0;
        slider.minValue = 0;
    }

    /**
     * Sets the maximum value for the slider
     * @param value The maximum fill value
     */
    public void setMaxValue(int value)
    {
        slider.maxValue = value;
    }

    /**
     * Instantly sets the slider to a specific value
     * @param value The target fill value
     */
    public void setFillValue(int value)
    {
        slider.value = value;
    }

    /**
     * Smoothly transitions the slider to a target value over time
     * @param targetValue The final fill value
     * @param duration Time in seconds for the transition
     */
    public void SetFillValueSmooth(int targetValue, float duration)
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }
        fillCoroutine = StartCoroutine(SmoothTransition(targetValue, duration));
    }

    /**
     * Coroutine that handles the smooth transition animation
     * @param targetValue The final fill value
     * @param duration Time in seconds for the transition
     */
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
