using UnityEngine;
using Leap.Unity;
using Leap;

/// <summary>
/// Translates Leap Motion hand tracking data into player movement inputs.
/// Detects palm position and converts it to a normalized movement vector.
/// </summary>
public class LeapMotionMovement : MonoBehaviour
{
    public Player playerScript;                    // Reference to the player controller
    public float movementSpeed = 1f;               // Multiplier for movement speed
    public float deadZoneRadius = 0.05f;           // Minimum hand movement before registering input
    public float maxOffset = 0.15f;                // Maximum hand offset for full speed

    private LeapProvider provider;                 // Reference to the Leap Motion data provider

    /// <summary>
    /// Initializes the component by finding the Leap Motion provider.
    /// </summary>
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
    }

    /// <summary>
    /// Processes hand tracking data every frame and converts it to movement input.
    /// </summary>
    void Update()
    {
        // Check if provider exists
        if (provider == null) return;

        var frame = provider.CurrentFrame;
        // If no frame data or no hands detected, stop movement
        if (frame == null || frame.Hands.Count == 0)
        {
            // No hand detected, stop player movement
            playerScript.SendMessage("MovePressed", Vector2.zero);
            return;
        }

        // Get the first detected hand
        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition;

        // Extract horizontal position (x and z axes)
        Vector2 offset = new Vector2(palmPosition.x, palmPosition.z);

        // Apply dead zone - if hand is close to center position, don't move
        if (offset.magnitude < deadZoneRadius)
        {
            playerScript.SendMessage("MovePressed", Vector2.zero);
        }
        else
        {
            // Normalize movement vector based on maximum offset and apply speed
            Vector2 clamped = Vector2.ClampMagnitude(offset / maxOffset, 1.0f);
            playerScript.SendMessage("MovePressed", clamped * movementSpeed);
        }
    }
}
