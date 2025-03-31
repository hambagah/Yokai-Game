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
using UnityEngine.Events;
using System;

public class BowlDetector : MonoBehaviour
{
    #region Inspector Fields

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

    // Events
    [Header("Events")]
    [Tooltip("Event fired when an ingredient is added or removed")]
    public UnityEvent onIngredientsChanged;

    [Tooltip("Event fired when the bowl is filled to the maximum level")]
    public UnityEvent onBowlFilled;

    #endregion

    #region Private Fields

    // Internal state
    private float currentFill = 0f;
    private float lastFillTime = 0f;
    private bool wasBowlFull = false;
    private int iceCubeCollisionCount = 0; // Track how many ice cubes are touching the bowl

    #endregion

    #region Unity Lifecycle Methods

    /**
     * Initialize bowl state and UI elements on start
     */
    void Start()
    {
        if (liquidFill != null)
        {
            liquidFill.gameObject.SetActive(false);
        }

        // Initialize the fill bar with correct min/max values once at start
        if (fillBar != null)
        {
            fillBar.setMinValue();
            fillBar.setMaxValue((int)maxFill);
            fillBar.setFillValue(0); // Start with zero fill
        }

        // Initialize other UI elements
        UpdateUI();
    }

    /**
     * Detects when objects first make contact with the bowl
     * Used to track when an ice cube enters the bowl
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            iceCubeCollisionCount++;
            UpdateIceCubeState(true);
            Debug.Log("Ice cube has touched the bowl. Count: " + iceCubeCollisionCount);
        }
    }

    /**
     * Monitors continuous contact with objects
     * Ensures ice cube state remains true while in contact
     */
    private void OnCollisionStay(Collision collision)
    {
        // No need to update every frame - handled by collision enter/exit
    }

    /**
     * Detects when objects stop making contact with the bowl
     * Used to track when an ice cube leaves the bowl
     */
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("IceCube"))
        {
            iceCubeCollisionCount = Mathf.Max(0, iceCubeCollisionCount - 1); // Ensure we don't go below 0
            
            if (iceCubeCollisionCount == 0)
            {
                UpdateIceCubeState(false);
                Debug.Log("All ice cubes have left the bowl.");
            }
            else
            {
                Debug.Log("Ice cube has left the bowl. Remaining: " + iceCubeCollisionCount);
            }
        }
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
                // Update fill level
                float previousFill = currentFill;
                currentFill += fillRate;
                currentFill = Mathf.Clamp(currentFill, 0, maxFill);
                lastFillTime = Time.time;

                // Update UI and track ingredient state
                UpdateFillBar();
                
                // Log ingredient and update UI text
                string ingredientName = other.tag;
                Debug.Log("You added: " + ingredientName);
                UpdateIngredientAddedText(ingredientName);
                
                // Update fill level text
                UpdateFillLevelText();

                // Check if bowl is now full
                CheckBowlFullStatus();

                // Update ingredient tracking
                bool ingredientAdded = false;
                if (other.CompareTag("Sake") && !hasSake)
                {
                    hasSake = true;
                    ingredientAdded = true;
                }
                else if (other.CompareTag("Juice") && !hasJuice)
                {
                    hasJuice = true;
                    ingredientAdded = true;
                }

                // Update UI toggles and trigger events if needed
                if (ingredientAdded || Mathf.Abs(currentFill - previousFill) > 0.01f)
                {
                    UpdateIngredientToggles();
                    onIngredientsChanged?.Invoke();
                }
            }
        }
    }

    #endregion

    #region Public Methods

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
     * Resets the bowl to its initial empty state
     * Useful for restarting the game
     */
    public void ResetBowl()
    {
        currentFill = 0;
        hasIceCube = false;
        hasSake = false;
        hasJuice = false;
        iceCubeCollisionCount = 0;
        wasBowlFull = false;
        
        UpdateFillBar();
        UpdateIngredientToggles();
        UpdateUI();
        
        onIngredientsChanged?.Invoke();
    }

    /**
     * Checks if all required ingredients are present and bowl is full
     * Returns true if all conditions are met for submitting the drink
     */
    public bool IsComplete()
    {
        return IsIceCubeInBowl() && IsSakeInBowl() && IsJuiceInBowl() && currentFill >= maxFill;
    }

    #endregion

    #region Private Helper Methods

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

    /**
     * Updates the fill bar UI element
     */
    private void UpdateFillBar()
    {
        if (fillBar != null)
        {
            // Ensure the fill bar game object is active
            if (!fillBar.gameObject.activeInHierarchy)
            {
                fillBar.gameObject.SetActive(true);
                Debug.Log("Activating Fill Bar that was inactive");
            }
            
            // Only initialize min/max values once, don't reset to zero each time
            fillBar.setMaxValue((int)maxFill);
            fillBar.SetFillValueSmooth((int)currentFill, 0.5f);
        }
    }

    /**
     * Updates the text showing the current fill level
     */
    private void UpdateFillLevelText()
    {
        if (fillLevelText != null)
        {
            if (currentFill >= maxFill)
            {
                fillLevelText.text = "The bowl is full!";
                Debug.Log("Bowl is full!");
            }
            else
            {
                fillLevelText.text = "Current fill level: " + Mathf.Round(currentFill);
                Debug.Log("Current fill level: " + currentFill);
            }
        }
    }

    /**
     * Updates the text showing what ingredient was just added
     */
    private void UpdateIngredientAddedText(string ingredientName)
    {
        if (ingredientAddingText != null)
        {
            ingredientAddingText.text = "You added: " + ingredientName;
        }
    }

    /**
     * Fires the onBowlFilled event if the bowl just became full
     */
    private void CheckBowlFullStatus()
    {
        bool isNowFull = currentFill >= maxFill;
        
        // Only fire the event when the bowl transitions from not full to full
        if (isNowFull && !wasBowlFull)
        {
            onBowlFilled?.Invoke();
        }
        
        wasBowlFull = isNowFull;
    }

    /**
     * Updates all UI elements
     */
    private void UpdateUI()
    {
        UpdateIngredientToggles();
        UpdateFillLevelText();
    }

    /**
     * Updates the ice cube state and triggers events if necessary
     */
    private void UpdateIceCubeState(bool newState)
    {
        if (hasIceCube != newState)
        {
            hasIceCube = newState;
            UpdateIngredientToggles();
            onIngredientsChanged?.Invoke();
        }
    }

    #endregion
}
