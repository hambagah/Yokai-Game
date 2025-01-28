using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BowlDetector : MonoBehaviour
{
    public float maxFill = 100f;  // Maximum fill level
    public float fillRate = 0.0001f;  // Amount added per particle collision (adjust to a smaller value for slower filling)
    public float timeToFill = 1f; // Time in seconds to add a fixed amount of fill (for slower filling)
    public Transform liquidFill; // Reference to liquid fill object for visualization (can be ignored for now)
    public bool hasIceCube = false; // To check if an ice cube is in contact with the bowl
    public bool hasSake = false;
    public bool hasJuice = false;
    private float currentFill = 0f;
    private float lastFillTime = 0f;
    public TextMeshProUGUI fillLevelText;
    public TextMeshProUGUI ingredientAddingText;
    public FillBar fillBar;

    // UI Toggles for Ingredients
    public Toggle iceCubeToggle;
    public Toggle sakeToggle;
    public Toggle juiceToggle;

    void Start()
    {
        currentFill = 0;
        fillBar.setMinValue();
        fillBar.setMaxValue((int)maxFill);

        // Initialize toggle states
        UpdateIngredientToggles();
    }

    // This method is called when an object first collides with the bowl
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            hasIceCube = true; // Ice cube has touched the bowl
            Debug.Log("Ice cube has touched the bowl.");
        }
    }

    // This method is called while the ice cube is still touching the bowl
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            hasIceCube = true; // Ice cube is still in contact with the bowl
        }
        UpdateIngredientToggles();
    }

    // This method is called when the ice cube stops touching the bowl
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            hasIceCube = false; // Ice cube has stopped touching the bowl
            Debug.Log("Ice cube has left the bowl.");
        }
        UpdateIngredientToggles();

    }

    // This method is called when a particle collides with the bowl
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Sake") || other.CompareTag("Juice")) // Ensure the pouring stream has the correct tag
        {
            // Check if enough time has passed to add fill to the bowl
            if (Time.time - lastFillTime >= timeToFill)
            {
                currentFill += fillRate; // Add fill based on the rate
                currentFill = Mathf.Clamp(currentFill, 0, maxFill); // Clamp within the bowl's max fill level

                // Update the fill bar smoothly
                fillBar.SetFillValueSmooth((int)currentFill, 0.5f); // Smooth transition over 0.5 seconds

                lastFillTime = Time.time; // Update the last fill time

                // Log the ingredient player is adding
                Debug.Log("You added: " + other.tag);
                ingredientAddingText.text = "You added: " + other.tag;

                // Log the current fill level
                Debug.Log("Current fill level: " + currentFill);
                fillLevelText.text = "Current fill level: " + currentFill;

                // Check if the bowl is full
                if (currentFill >= maxFill)
                {
                    Debug.Log("Bowl is full!");
                    fillLevelText.text = "The bowl is full!";
                }
                else
                {
                    Debug.Log("Bowl is not full yet.");
                }

                if (other.CompareTag("Sake"))
                {
                    hasSake = true;
                }
                else if (other.CompareTag("Juice"))
                {
                    hasJuice = true;
                }
                UpdateIngredientToggles();
            }
        }
    }

    // This method is useful to get the current fill level externally
    public float GetFillLevel()
    {
        return currentFill; // Return current fill level for game logic
    }

    // You can also call this method to check if ice is in the bowl
    public bool IsIceCubeInBowl()
    {
        return hasIceCube; // Return true if the ice cube is in contact with the bowl
    }

    public bool IsSakeInBowl()
    {
        return hasSake;
    }
    public bool IsJuiceInBowl()
    {
        return hasJuice;
    }

    private void UpdateIngredientToggles()
    {
        // Update the state of each toggle based on ingredient flags
        if (iceCubeToggle) iceCubeToggle.isOn = hasIceCube;
        if (sakeToggle) sakeToggle.isOn = hasSake;
        if (juiceToggle) juiceToggle.isOn = hasJuice;
    }
}
