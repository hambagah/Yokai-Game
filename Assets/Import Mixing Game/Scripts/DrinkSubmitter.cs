/**
 * DrinkSubmitter.cs
 * 
 * Summary: Handles the submission of the drink, either manually or
 * automatically when the timer ends. Controls the game flow by initiating
 * the timer and evaluating the results when submitted.
 */
using UnityEngine;
using UnityEngine.UI;

public class DrinkSubmitter : MonoBehaviour
{
    [Tooltip("Reference to the win condition checker")]
    public DemoWinCheck demoWinCheck;
    
    [Tooltip("Reference to the game timer")]
    public MixingGameTimer timer;

    // Track if the drink has been submitted to prevent multiple submissions
    private bool isSubmitted = false;

    /**
     * Initializes event listeners for timer end
     */
    private void Start()
    {
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(AutoSubmitDrink);
        }
    }

    /**
     * Begins the timer for the mixing game
     * Called when the player begins the challenge
     */
    public void StartTimer()
    {
        if (timer != null)
        {
            timer.StartTimer();
            Debug.Log("Timer started!");
        }
    }

    /**
     * Manually submits the drink for evaluation
     * Called by UI button press or other game events
     */
    public void SubmitDrink()
    {
        if (isSubmitted) return; // Prevent multiple submissions
        isSubmitted = true;

        Debug.Log("Drink submitted!");
        if (demoWinCheck != null)
        {
            demoWinCheck.CheckWinCondition();
        }
    }

    /**
     * Automatically submits the drink when time runs out
     * Called by timer event
     */
    public void AutoSubmitDrink()
    {
        if (!isSubmitted)
        {
            Debug.Log("Time ran out! Auto-submitting drink...");
            SubmitDrink();
        }
    }

    /**
     * Adds timer event listener when script is enabled
     */
    private void OnEnable()
    {
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(AutoSubmitDrink);
        }
    }

    /**
     * Removes timer event listener when script is disabled
     */
    private void OnDisable()
    {
        if (timer != null)
        {
            timer.onTimerEnd.RemoveListener(AutoSubmitDrink);
        }
    }
}
