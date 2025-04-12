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
    public float triggerVelocity = 1.2f;           // Minimum vertical velocity to trigger the action
    public float cooldownTime = 1.0f;              // Time between allowed triggers to prevent spam

    private LeapProvider provider;                 // Reference to the Leap Motion data provider
    private float lastTriggerTime = -Mathf.Infinity; // Timestamp of the last triggered action
    private float previousY = 0f;                  // Previous Y position of hand for velocity calculation
    private bool initialized = false;              // Flag to ensure we have valid previous position data

    /// <summary>
    /// Initializes the component by finding the Leap Motion provider.
    /// </summary>
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
    }

    /// <summary>
    /// Monitors hand movement each frame to detect the enter/submit gesture.
    /// </summary>
    void Update()
    {
        // Skip checking if provider is missing or we're still in cooldown period
        if (provider == null || Time.time - lastTriggerTime < cooldownTime) return;

        var frame = provider.CurrentFrame;
        if (frame.Hands.Count == 0)
        {
            // Reset if no hands detected
            initialized = false;
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
        previousY = currentY;

        // Trigger submit action regardless of dialogue choice status
        // The DialogueManager will handle the context appropriately
        if (Mathf.Abs(velocityY) > triggerVelocity)
        {
            // Always use the same event trigger - DialogueManager will handle the context
            Debug.Log("Trigger");
            GameEventsManager.instance.inputEvents.SubmitPressed();
            Debug.Log("Leap ENTER gesture triggered with velocity: " + velocityY);
            lastTriggerTime = Time.time;
        }
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