using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Detects vertical hand gestures using Leap Motion and triggers the Submit/Enter action.
/// Includes cooldown and safety checks to prevent accidental triggers during dialogue choices.
/// </summary>
public class LeapEnterGestureTrigger : MonoBehaviour
{
    [Header("Gesture Settings")]
    public float triggerVelocity = 1.2f;           // Minimum vertical velocity to trigger the action
    public float cooldownTime = 1.0f;              // Time between allowed triggers to prevent spam
    public float gestureCompletionTime = 0.3f;     // Time to wait for a gesture to complete
    public float minVelocityDelta = 10.0f;         // Minimum change in velocity to detect a new gesture
    
    [Header("Debug")]
    public bool enableDebugLogs = true;            // Enable detailed debug logging
    public bool showVelocity = false;              // Show velocity every frame for debugging

    private LeapProvider provider;                 // Reference to the Leap Motion data provider
    private float lastTriggerTime = -Mathf.Infinity; // Timestamp of the last triggered action
    private float previousY = 0f;                  // Previous Y position of hand for velocity calculation
    private bool initialized = false;              // Flag to ensure we have valid previous position data
    private bool gestureInProgress = false;        // Track if we're in the middle of a gesture
    private float gestureStartTime = 0f;           // When the current gesture started
    private float lastMaxVelocity = 0f;            // Track the last max velocity for a gesture
    private float lastTriggerY = 0f;               // The Y position at which the last trigger occurred
    private int framesSinceDirectionChange = 0;    // Count frames since direction changed

    /// <summary>
    /// Initializes the component by finding the Leap Motion provider.
    /// </summary>
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
        if (provider == null)
        {
            Debug.LogError("LeapEnterGestureTrigger: No LeapProvider found in scene!");
        }
    }

    /// <summary>
    /// Monitors hand movement each frame to detect the enter/submit gesture.
    /// </summary>
    void Update()
    {
        // Skip checking if provider is missing
        if (provider == null) return;

        var frame = provider.CurrentFrame;
        if (frame.Hands.Count == 0)
        {
            // Reset if no hands detected
            initialized = false;
            gestureInProgress = false;
            framesSinceDirectionChange = 0;
            return;
        }

        // Get the first detected hand
        var hand = frame.Hands[0];
        float currentY = hand.PalmPosition.y;

        // First frame of detection - just store the position
        if (!initialized)
        {
            previousY = currentY;
            initialized = true;
            return;
        }

        // Calculate vertical velocity
        float velocityY = (currentY - previousY) / Time.deltaTime;
        
        // Show velocity for debugging if enabled
        if (showVelocity)
        {
            Debug.Log($"Current velocity: {velocityY:F2}");
        }

        // Handle gesture state
        if (Time.time - lastTriggerTime < cooldownTime)
        {
            // Skip further processing during cooldown
            previousY = currentY;
            return;
        }
        
        // Check for significant downward motion
        if (!gestureInProgress && velocityY < -triggerVelocity)
        {
            // Start a new gesture - significant downward motion detected
            gestureInProgress = true;
            gestureStartTime = Time.time;
            lastMaxVelocity = velocityY;
            lastTriggerY = currentY;
            
            // Trigger the submit action
            GameEventsManager.instance.inputEvents.SubmitPressed();
            if (enableDebugLogs)
            {
                Debug.Log($"Leap ENTER gesture triggered with velocity: {velocityY:F2}");
            }
            lastTriggerTime = Time.time;
            
            // Force cooldown to prevent multiple triggers
            previousY = currentY;
            return;
        }
        
        // Handle gesture completion
        if (gestureInProgress)
        {
            // A gesture is considered complete when:
            // 1. Direction changes (upward motion after downward)
            // 2. Velocity drops significantly
            // 3. Time elapsed is sufficient
            
            bool directionChanged = velocityY > 0;  // Changed from downward to upward
            bool velocityDropped = Mathf.Abs(velocityY) < triggerVelocity * 0.5f; // Velocity reduced significantly
            bool timeElapsed = Time.time - gestureStartTime > gestureCompletionTime;
            
            if (directionChanged || velocityDropped || timeElapsed)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"Gesture ended: Direction changed: {directionChanged}, " +
                              $"Velocity dropped: {velocityDropped}, Time elapsed: {timeElapsed}");
                }
                gestureInProgress = false;
                framesSinceDirectionChange = 0;
            }
        }
        
        // Update for next frame
        previousY = currentY;
    }

    /// <summary>
    /// Checks if any dialogue choice buttons are currently active in the UI.
    /// Used to prevent accidental triggers when making choices.
    /// </summary>
    /// <returns>True if any choice buttons are active, false otherwise</returns>
    private bool IsChoiceButtonsActive()
    {
        GameObject choices = GameObject.Find("DialogueChoices");
        if (choices == null || !choices.activeInHierarchy) return false;

        foreach (Transform child in choices.transform)
        {
            if (child.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the player is currently in a dialogue interaction.
    /// </summary>
    /// <returns>True if dialogue UI is active, false otherwise</returns>
    private bool IsInDialogue()
    {
        GameObject dialogueCanvas = GameObject.Find("DialogueCanvas");
        return dialogueCanvas != null && dialogueCanvas.activeInHierarchy;
    }
}