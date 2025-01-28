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
        // Assuming the timer's onTimerEnd event is used to trigger the check
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(CheckRequirements);
        }
    }

    private void CheckRequirements()
    {
        // Check if ice cube is in the bowl and it's filled with sake
        if (bowlDetector.IsIceCubeInBowl() && bowlDetector.IsSakeInBowl() && bowlDetector.IsJuiceInBowl())
        {
            if (bowlDetector.GetFillLevel() >= bowlDetector.maxFill)
            {
                FinishText.text = "Great success!\n Ice cube is in the bowl, you have got the right ingredients and the bowl is full!";
                FinishText.color = Color.green;
                Debug.Log("Great success!\n Ice cube is in the bowl, you have got the right ingredients and the bowl is full!");
            }
            else
            {
                Debug.Log("Requirements met!\n Ice cube is in, you have got the right ingredients and the bowl is " + bowlDetector.GetFillLevel() + "% full.");
                FinishText.color = Color.gray;
                FinishText.text = "Requirements met!\n Ice cube is in, you have got the right ingredients and the bowl is " + bowlDetector.GetFillLevel() + "% full.";
            }

        }
        else
        {
            if (!bowlDetector.IsIceCubeInBowl() && !bowlDetector.IsSakeInBowl() && !bowlDetector.IsJuiceInBowl())
            {
                Debug.Log("Critical fail.\n Nothing is in the bowl.");
                FinishText.color = Color.red;
                FinishText.text = "Critical fail.\n Nothing is in the bowl.";

            }
            else
            {
                Debug.Log("Fail.");
                FinishText.color = Color.red;
                string tempFinishText = "Fail!";

                if (!bowlDetector.IsIceCubeInBowl())
                {
                    Debug.Log("\nMissing Ice cube.");

                    tempFinishText += "\nMissing Ice cube.";
                }
                if (!bowlDetector.IsSakeInBowl())
                {
                    Debug.Log("\nMissing sake.");

                    tempFinishText += "\nMissing sake.";
                }

                if (!bowlDetector.IsJuiceInBowl())
                {
                    Debug.Log("\nMissing juice.");

                    tempFinishText += "\nMissing juice.";
                }

                FinishText.text = tempFinishText;

            }

        }
    }
}
