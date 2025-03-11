using UnityEngine;
using UnityEngine.UI;

public class DrinkSubmitter : MonoBehaviour
{
    public DemoWinCheck demoWinCheck;
    public MixingGameTimer timer; // Reference to your timer script

    private bool isSubmitted = false; // Prevent multiple submissions

    private void Start()
    {
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(AutoSubmitDrink);
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

        Debug.Log("Drink submitted!");
        if (demoWinCheck != null)
        {
            demoWinCheck.CheckWinCondition();
        }
    }

    public void AutoSubmitDrink()
    {
        if (!isSubmitted)
        {
            Debug.Log("Time ran out! Auto-submitting drink...");
            SubmitDrink();
        }
    }

    private void OnEnable()
    {
        timer.onTimerEnd.AddListener(AutoSubmitDrink);
    }

    private void OnDisable()
    {
        timer.onTimerEnd.RemoveListener(AutoSubmitDrink);
    }
}
