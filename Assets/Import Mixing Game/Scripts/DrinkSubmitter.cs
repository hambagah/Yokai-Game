using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.PhysicalHands;

public class DrinkSubmitter : MonoBehaviour
{
    public DemoWinCheck demoWinCheck;
    public PhysicalHandsButton submitButton; // Reference to Leap Motion-controlled button

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.OnButtonPressed.AddListener(SubmitDrink);
        }
    }

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.OnButtonPressed.RemoveListener(SubmitDrink);
        }
    }


    private void SubmitDrink()
    {
        Debug.Log("Drink submitted!");

        if (demoWinCheck == null)
        {
            Debug.LogError("DrinkSubmitter: demoWinCheck reference is missing!");
            return;
        }

        demoWinCheck.CheckWinCondition();
    }


}
