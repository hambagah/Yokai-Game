using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

public class DemoWinCheck : MonoBehaviour
{
    public MixingGameTimer timer; // Reference to the timer
    public BowlDetector bowlDetector; // Reference to the bowl detection script

    public TextMeshProUGUI FinishText;

    private void Start()
    {
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(CheckRequirements);
        }

        if (bowlDetector == null)
        {
            bowlDetector = FindObjectOfType<BowlDetector>();
            if (bowlDetector == null)
            {
                Debug.LogError("DemoWinCheck: bowlDetector is NULL! Ensure it is assigned in the Inspector.");
            }
        }

        if (FinishText == null)
        {
            Debug.LogError("DemoWinCheck: FinishText is NULL! Assign a UI text element in the Inspector.");
        }
    }


    private void CheckRequirements()
    {
        if (bowlDetector == null)
        {
            Debug.LogError("DemoWinCheck: bowlDetector is NULL!");
            return;
        }
        if (FinishText == null)
        {
            Debug.LogError("DemoWinCheck: FinishText is NULL!");
            return;
        }

        // Check if the required ingredients are in the bowl
        if (bowlDetector.IsIceCubeInBowl() && bowlDetector.IsSakeInBowl() && bowlDetector.IsJuiceInBowl())
        {
            if (bowlDetector.GetFillLevel() >= bowlDetector.maxFill)
            {
                FinishText.text = "Great success!\n The bowl is full with the correct ingredients!";
                FinishText.color = Color.green;
                Debug.Log(FinishText.text);
            }
            else
            {
                FinishText.text = $"Requirements met!\n The bowl is {bowlDetector.GetFillLevel()}% full.";
                FinishText.color = Color.gray;
                Debug.Log(FinishText.text);
            }
        }
        else
        {
            string failMessage = "Fail!";
            if (!bowlDetector.IsIceCubeInBowl()) failMessage += "\nMissing Ice cube.";
            if (!bowlDetector.IsSakeInBowl()) failMessage += "\nMissing sake.";
            if (!bowlDetector.IsJuiceInBowl()) failMessage += "\nMissing juice.";
            FinishText.text = failMessage;
            FinishText.color = Color.red;
            Debug.Log(FinishText.text);
        }
    }


    public void CheckWinCondition()
    {
        CheckRequirements(); // Call the existing CheckRequirements method
    }

}
