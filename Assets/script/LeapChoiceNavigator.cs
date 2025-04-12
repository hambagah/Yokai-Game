using UnityEngine;
using UnityEngine.EventSystems;
using Leap;
using Leap.Unity;

/// <summary>
/// Handles navigation between dialogue choices using Leap Motion hand gestures.
/// Detects horizontal swipe gestures to change the selected dialogue option.
/// </summary>
public class LeapChoiceNavigator : MonoBehaviour
{
    [Tooltip("Velocity threshold to detect a swipe gesture")]
    public float swipeThreshold = 0.7f; // Minimum velocity to register a swipe
    
    [Tooltip("Time in seconds before another swipe can be detected")]
    public float cooldown = 0.5f;       // Prevents accidental multiple swipes

    private LeapProvider provider;       // Reference to the Leap Motion data provider
    private float lastSwipeTime = -Mathf.Infinity;  // Timestamp of the last detected swipe
    private int currentIndex = 0;        // Current index of the selected dialogue choice

    /// <summary>
    /// Initialize by finding the Leap Motion provider
    /// </summary>
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
    }

    /// <summary>
    /// Detect swipe gestures and change the selected dialogue option accordingly
    /// </summary>
    void Update()
    {
        // Skip if provider is missing or in cooldown period
        if (provider == null || Time.time - lastSwipeTime < cooldown) return;

        var frame = provider.CurrentFrame;
        // Skip if no hands are detected
        if (frame.Hands.Count == 0) return;

        var hand = frame.Hands[0];
        float xVel = hand.PalmVelocity.x;

        // Check if the horizontal velocity exceeds the swipe threshold
        if (Mathf.Abs(xVel) > swipeThreshold)
        {
            bool right = xVel > 0; // Determine swipe direction (right or left)

            // Find the current selected object and its parent container
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == null) return;

            Transform parent = currentSelected.transform.parent;
            int siblingCount = parent.childCount;

            // Count only active choice buttons
            int activeCount = 0;
            foreach (Transform child in parent)
            {
                if (child.gameObject.activeInHierarchy) activeCount++;
            }

            // Determine next index based on swipe direction
            if (right)
                currentIndex = (currentIndex + 1) % activeCount; // Move to next option
            else
                currentIndex = (currentIndex - 1 + activeCount) % activeCount; // Move to previous option

            // Update the UI selection to the new choice
            int activeIndex = 0;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (!parent.GetChild(i).gameObject.activeInHierarchy) continue;

                if (activeIndex == currentIndex)
                {
                    // Select the appropriate UI element
                    EventSystem.current.SetSelectedGameObject(parent.GetChild(i).gameObject);
                    
                    // Update the dialogue system's current choice index
                    GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(activeIndex);
                    break;
                }
                activeIndex++;
            }

            // Start cooldown period
            lastSwipeTime = Time.time;
        }
    }
}
