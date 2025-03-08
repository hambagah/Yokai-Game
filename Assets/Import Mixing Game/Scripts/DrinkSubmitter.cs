using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.PhysicalHands;

public class DrinkSubmitter : MonoBehaviour
{
    public MixingGameTimer timer; // Timer reference
    public DemoWinCheck demoWinCheck;
    public PhysicalHandsButton submitButton; // Leap Motion-controlled button

    private bool isSubmitted = false; // Prevent multiple submissions

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.OnButtonPressed.AddListener(SubmitDrink);
        }

        if (timer != null)
        {
            timer.onTimerEnd.AddListener(AutoSubmitDrink); // ✅ Correctly adding event
        }
    }

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.OnButtonPressed.RemoveListener(SubmitDrink);
        }

        if (timer != null)
        {
            timer.onTimerEnd.RemoveListener(AutoSubmitDrink); // ✅ Fixed line 31!
        }
    }

    public void StartTimer()
    {
        if (timer != null)
        {
            timer.StartTimer();
            Debug.Log("Timer started!");
        }
    }

    public void SubmitDrink()
    {
        if (isSubmitted) return; // Prevent multiple submissions
        isSubmitted = true;

        if (timer != null)
        {
            timer.StopTimer(); // Stop the timer when submitting manually
        }

        if (demoWinCheck != null)
        {
            demoWinCheck.CheckWinCondition(); // Evaluate the drink
        }

        Debug.Log("Drink submitted!");
    }

    private void AutoSubmitDrink()
    {
        if (!isSubmitted)
        {
            Debug.Log("Time ran out! Auto-submitting drink...");
            SubmitDrink();
        }
    }
}
