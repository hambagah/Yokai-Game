using UnityEngine;
using TMPro;

public class DemoWinCheck : MonoBehaviour
{
    public BowlDetector bowlDetector; // Reference to BowlDetector for checking ingredients
    public TextMeshProUGUI FinishText; // UI Text to display the result

    public void CheckWinCondition()
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

        // Check if ingredients are correct
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
}
