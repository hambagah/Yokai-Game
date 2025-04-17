/**
 * DrinkSubmitter.cs
 * 
 * Summary: Handles the submission of the drink, either manually or
 * automatically when the timer ends. Controls the game flow by initiating
 * the timer and evaluating the results when submitted.
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrinkSubmitter : MonoBehaviour
{
    [Tooltip("Reference to the win condition checker")]
    public DemoWinCheck demoWinCheck;
    
    [Tooltip("Reference to the game timer")]
    public MixingGameTimer timer;

    [Tooltip("Reference to the bowl detector script")]
    public BowlDetector bowlDetector;

    [Tooltip("Reference to the submit button that should be enabled/disabled")]
    public Button submitButton;

    [Tooltip("How frequently to check drink completion status (in seconds)")]
    public float checkInterval = 0.5f;

    // Track if the drink has been submitted to prevent multiple submissions
    private bool isSubmitted = false;
    private float checkTimer = 0f;

    /**
     * Initializes event listeners for timer end and disables submit button initially
     */
    private void Start()
    {
        // Ensure we have an active EventSystem
        EnsureEventSystem();
        
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(AutoSubmitDrink);
        }

        // Verify that all required components are assigned
        if (bowlDetector == null)
        {
            Debug.LogError("DrinkSubmitter: BowlDetector reference is missing!");
        }

        if (submitButton == null)
        {
            Debug.LogError("DrinkSubmitter: Submit Button reference is missing!");
        }
        else
        {
            // Hide the submit button at the start of the game
            submitButton.gameObject.SetActive(false);
            // Make sure the canvas is enabled and configured for input
            EnsureCanvasIsConfigured(submitButton.gameObject);
        }
    }

    /**
     * Makes sure there's an active EventSystem
     */
    private void EnsureEventSystem()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>(true);
        if (systems.Length == 0)
        {
            Debug.LogWarning("No EventSystem found in scene! Creating one...");
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        else if (!systems[0].gameObject.activeInHierarchy)
        {
            Debug.LogWarning("EventSystem found but inactive, activating it");
            systems[0].gameObject.SetActive(true);
        }
    }

    /**
     * Ensures the UI Canvas is properly configured for input
     */
    private void EnsureCanvasIsConfigured(GameObject buttonObject)
    {
        Canvas canvas = buttonObject.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            // Make sure the canvas has a GraphicRaycaster
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.LogWarning("Canvas missing GraphicRaycaster, adding one");
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Debug.Log("Canvas is using ScreenSpaceOverlay mode, this should work for UI interactions");
            }
            else
            {
                Debug.LogWarning("Canvas is not using ScreenSpaceOverlay mode, this might cause input issues");
            }
        }
        else
        {
            Debug.LogError("Submit button is not under a Canvas hierarchy!");
        }
    }

    /**
     * Periodically checks if the drink requirements are met to enable/disable the submit button
     */
    private void Update()
    {
        checkTimer += Time.deltaTime;

        // Check drink status at regular intervals to avoid doing it every frame
        if (checkTimer >= checkInterval && !isSubmitted)
        {
            checkTimer = 0f;
            UpdateSubmitButtonState();
        }
    }

    /**
     * Checks if all conditions are met and updates the submit button state accordingly
     */
    private void UpdateSubmitButtonState()
    {
        if (bowlDetector == null || submitButton == null) 
        {
            Debug.LogWarning("DrinkSubmitter: Missing references for button update");
            return;
        }

        // Check if all required conditions are met
        bool allIngredientsPresent = bowlDetector.IsIceCubeInBowl() && 
                                    bowlDetector.IsSakeInBowl() && 
                                    bowlDetector.IsJuiceInBowl();
        bool isFull = bowlDetector.GetFillLevel() >= bowlDetector.maxFill;
        bool isDrinkComplete = allIngredientsPresent && isFull;
        
        // Log the state for debugging
        Debug.Log($"Drink state: Ice={bowlDetector.IsIceCubeInBowl()}, " +
                  $"Sake={bowlDetector.IsSakeInBowl()}, " +
                  $"Juice={bowlDetector.IsJuiceInBowl()}, " +
                  $"Fill={bowlDetector.GetFillLevel()}/{bowlDetector.maxFill}, " +
                  $"Drink complete={isDrinkComplete}");

        // Only show the button when the drink is complete
        if (isDrinkComplete)
        {
            if (!submitButton.gameObject.activeSelf)
            {
                Debug.Log("Activating submit button - drink is complete!");
                submitButton.gameObject.SetActive(true);
            }
            if (!submitButton.interactable)
            {
                Debug.Log("Making submit button interactable");
                submitButton.interactable = true;
            }
        }
        else
        {
            if (submitButton.gameObject.activeSelf)
            {
                Debug.Log("Deactivating submit button - drink not complete");
                submitButton.gameObject.SetActive(false);
            }
        }
    }

    /**
     * Begins the timer for the mixing game
     * Called when the player begins the challenge
     */
    public void StartTimer()
    {
        if (timer != null)
        {
            timer.StartTimer();
            Debug.Log("Timer started!");
        }
    }

    /**
     * Manually submits the drink for evaluation
     * Called by UI button press or other game events
     */
    public void SubmitDrink()
    {
        Debug.Log("SubmitDrink method called");
        
        if (isSubmitted)
        {
            Debug.Log("Drink already submitted, ignoring");
            return; // Prevent multiple submissions
        }
        
        isSubmitted = true;
        Debug.Log("Drink submitted!");
        
        if (demoWinCheck != null)
        {
            demoWinCheck.CheckWinCondition();
        }
        else
        {
            Debug.LogError("demoWinCheck is null!");
        }

        // Disable the submit button after submission
        if (submitButton != null)
        {
            submitButton.interactable = false;
        }
    }

    /**
     * Automatically submits the drink when time runs out
     * Called by timer event
     */
    public void AutoSubmitDrink()
    {
        if (!isSubmitted)
        {
            Debug.Log("Time ran out! Auto-submitting drink...");
            SubmitDrink();
        }
    }

    /**
     * Adds timer event listener when script is enabled
     */
    private void OnEnable()
    {
        if (timer != null)
        {
            timer.onTimerEnd.AddListener(AutoSubmitDrink);
        }
        
        if (bowlDetector != null)
        {
            bowlDetector.onIngredientsChanged.AddListener(UpdateSubmitButtonState);
            bowlDetector.onBowlFilled.AddListener(UpdateSubmitButtonState);
        }
    }

    /**
     * Removes timer event listener when script is disabled
     */
    private void OnDisable()
    {
        if (timer != null)
        {
            timer.onTimerEnd.RemoveListener(AutoSubmitDrink);
        }
        
        if (bowlDetector != null)
        {
            bowlDetector.onIngredientsChanged.RemoveListener(UpdateSubmitButtonState);
            bowlDetector.onBowlFilled.RemoveListener(UpdateSubmitButtonState);
        }
    }

    /**
     * Resets the submission state, useful when restarting the game
     */
    public void ResetSubmissionState()
    {
        isSubmitted = false;
        
        // Hide the button when resetting
        if (submitButton != null)
        {
            submitButton.gameObject.SetActive(false);
            UpdateSubmitButtonState();
        }
    }
}
