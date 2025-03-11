/**
 * BowlDetector.cs
 * 
 * Summary: Detects and tracks ingredients added to a bowl in the mixing game.
 * This script monitors collisions with ice cubes and particle collisions from
 * liquid streams (sake, juice), updating UI elements to show current ingredients
 * and fill level.
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BowlDetector : MonoBehaviour
{
    // Configuration Parameters
    [Header("Fill Settings")]
    [Tooltip("Maximum fill level of the bowl")]
    public float maxFill = 100f;
    
    [Tooltip("Amount added per particle collision")]
    public float fillRate = 0.0001f;
    
    [Tooltip("Time in seconds between fill increments")]
    public float timeToFill = 1f;
    
    [Tooltip("Reference to liquid visualization object")]
    public Transform liquidFill;

    // Ingredient State Tracking
    [Header("Ingredient Tracking")]
    [Tooltip("True when an ice cube is in the bowl")]
    public bool hasIceCube = false;
    
    [Tooltip("True when sake has been added")]
    public bool hasSake = false;
    
    [Tooltip("True when juice has been added")]
    public bool hasJuice = false;

    // UI References
    [Header("UI References")]
    [Tooltip("Text showing current fill level")]
    public TextMeshProUGUI fillLevelText;
    
    [Tooltip("Text showing what ingredient is being added")]
    public TextMeshProUGUI ingredientAddingText;
    
    [Tooltip("Fill bar visual indicator")]
    public FillBar fillBar;

    // UI Toggles for Ingredients
    [Header("Ingredient UI Toggles")]
    [Tooltip("Toggle showing ice cube status")]
    public Toggle iceCubeToggle;
    
    [Tooltip("Toggle showing sake status")]
    public Toggle sakeToggle;
    
    [Tooltip("Toggle showing juice status")]
    public Toggle juiceToggle;

    // Internal state
    private float currentFill = 0f;
    private float lastFillTime = 0f;

    /**
     * Initialize bowl state and UI elements on start
     */
    void Start()
    {
        currentFill = 0;
        fillBar.setMinValue();
        fillBar.setMaxValue((int)maxFill);
        UpdateIngredientToggles();
    }

    /**
     * Detects when objects first make contact with the bowl
     * Used to track when an ice cube enters the bowl
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            hasIceCube = true;
            Debug.Log("Ice cube has touched the bowl.");
        }
    }

    /**
     * Monitors continuous contact with objects
     * Ensures ice cube state remains true while in contact
     */
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            hasIceCube = true;
        }
        UpdateIngredientToggles();
    }

    /**
     * Detects when objects stop making contact with the bowl
     * Used to track when an ice cube leaves the bowl
     */
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            hasIceCube = false;
            Debug.Log("Ice cube has left the bowl.");
        }
        UpdateIngredientToggles();
    }

    /**
     * Detects particle collisions from pouring liquids
     * Updates fill level and ingredient tracking for sake and juice
     */
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Sake") || other.CompareTag("Juice"))
        {
            // Check if enough time has passed to add fill
            if (Time.time - lastFillTime >= timeToFill)
            {
                currentFill += fillRate;
                currentFill = Mathf.Clamp(currentFill, 0, maxFill);

                // Update UI
                fillBar.SetFillValueSmooth((int)currentFill, 0.5f);
                lastFillTime = Time.time;

                // Log ingredient and update UI text
                Debug.Log("You added: " + other.tag);
                ingredientAddingText.text = "You added: " + other.tag;
                Debug.Log("Current fill level: " + currentFill);
                fillLevelText.text = "Current fill level: " + currentFill;

                // Check if bowl is full
                if (currentFill >= maxFill)
                {
                    Debug.Log("Bowl is full!");
                    fillLevelText.text = "The bowl is full!";
                }
                else
                {
                    Debug.Log("Bowl is not full yet.");
                }

                // Update ingredient tracking
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

    /**
     * Returns the current fill level of the bowl
     * Used by other scripts to check progress
     */
    public float GetFillLevel()
    {
        return currentFill;
    }

    /**
     * Checks if an ice cube is currently in the bowl
     * Used by other scripts to verify ingredients
     */
    public bool IsIceCubeInBowl()
    {
        return hasIceCube;
    }

    /**
     * Checks if sake has been added to the bowl
     * Used by other scripts to verify ingredients
     */
    public bool IsSakeInBowl()
    {
        return hasSake;
    }

    /**
     * Checks if juice has been added to the bowl
     * Used by other scripts to verify ingredients
     */
    public bool IsJuiceInBowl()
    {
        return hasJuice;
    }

    /**
     * Updates UI toggles to reflect current ingredient state
     * Called whenever ingredient state changes
     */
    private void UpdateIngredientToggles()
    {
        if (iceCubeToggle) iceCubeToggle.isOn = hasIceCube;
        if (sakeToggle) sakeToggle.isOn = hasSake;
        if (juiceToggle) juiceToggle.isOn = hasJuice;
    }
}
