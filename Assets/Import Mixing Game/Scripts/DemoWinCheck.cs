/**
 * DemoWinCheck.cs
 * 
 * Summary: Evaluates the win conditions for the drink mixing game.
 * Checks if the bowl contains all required ingredients (ice cube, sake, juice)
 * and if it has reached the required fill level, then updates UI text to show
 * success or failure.
 */
using UnityEngine;
using TMPro;

public class DemoWinCheck : MonoBehaviour
{
    [Tooltip("Reference to the bowl detector script")]
    public BowlDetector bowlDetector;
    
    [Tooltip("UI text that displays the result")]
    public TextMeshProUGUI FinishText;

    /**
     * Checks if all winning conditions are met and updates UI text accordingly
     * Called when the player submits their drink
     */
    public void CheckWinCondition()
    {
        // Safety checks for missing references
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

        // Check if all ingredients are present
        if (bowlDetector.IsIceCubeInBowl() && bowlDetector.IsSakeInBowl() && bowlDetector.IsJuiceInBowl())
        {
            // Check if fill level requirement is met
            if (bowlDetector.GetFillLevel() >= bowlDetector.maxFill)
            {
                // Perfect success
                FinishText.text = "Great success!\n The bowl is full with the correct ingredients!";
                FinishText.color = Color.green;
                Debug.Log(FinishText.text);
            }
            else
            {
                // Partial success - ingredients correct but not full
                FinishText.text = $"Requirements met!\n The bowl is {bowlDetector.GetFillLevel()}% full.";
                FinishText.color = Color.gray;
                Debug.Log(FinishText.text);
            }
        }
        else
        {
            // Failure - missing ingredients
            string failMessage = "Fail!";
            if (!bowlDetector.IsIceCubeInBowl()) failMessage += "\nMissing Ice cube.";
            if (!bowlDetector.IsSakeInBowl()) failMessage += "\nMissing sake.";
            if (!bowlDetector.IsJuiceInBowl()) failMessage += "\nMissing juice.";
            FinishText.text = failMessage;
            FinishText.color = Color.red;
            Debug.Log(FinishText.text);
        }
    }
}
