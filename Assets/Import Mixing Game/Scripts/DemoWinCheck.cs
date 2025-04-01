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
    public BowlDetector bowlDetector;
    public TextMeshProUGUI FinishText;
    public GameObject FinishPanel; // UI panel to show/hide along with finish text

    private void Start()
    {
        if (FinishPanel != null)
        {
            FinishPanel.SetActive(false); // Make sure it's initially off
        }
    }

    public void CheckWinCondition()
    {
        if (bowlDetector == null || FinishText == null || FinishPanel == null)
        {
            Debug.LogError("DemoWinCheck: Missing references!");
            return;
        }

        FinishPanel.SetActive(true);

        if (bowlDetector.IsIceCubeInBowl() && bowlDetector.IsSakeInBowl() && bowlDetector.IsJuiceInBowl())
        {
            if (bowlDetector.GetFillLevel() >= bowlDetector.maxFill)
            {
                FinishText.text = "Great success!\n The bowl is full with the correct ingredients!";
                FinishText.color = Color.green;
            }
            else
            {
                FinishText.text = $"Requirements met!\n The bowl is {bowlDetector.GetFillLevel()}% full.";
                FinishText.color = Color.black;
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
        }

        Debug.Log(FinishText.text);
    }

    public void HideFinishPanel()
    {
        if (FinishPanel != null)
        {
            FinishPanel.SetActive(false);
        }
    }
}
