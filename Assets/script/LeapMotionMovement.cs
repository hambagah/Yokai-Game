using UnityEngine;
using Leap.Unity;
using Leap;
using System.Collections;

/// <summary>
/// Translates Leap Motion hand tracking data into player movement inputs.
/// Detects palm position and converts it to a normalized movement vector.
/// </summary>
public class LeapMotionMovement : MonoBehaviour
{
    public Player playerScript;                    // Reference to the player controller
    
    [Header("Movement Settings")]
    public float movementSpeed = 1f;               // Multiplier for movement speed
    public float deadZoneRadius = 0.05f;           // Minimum hand movement before registering input
    public float maxOffset = 0.15f;                // Maximum hand offset for full speed
    
    [Header("Calibration")]
    public Vector2 centerOffset = Vector2.zero;    // Correction offset for the center position
    public bool useCalibration = true;             // Whether to use the calibrated center
    public KeyCode calibrateKey = KeyCode.C;       // Key to press for calibration
    public bool autoCalibrate = true;              // Whether to auto-calibrate on startup
    public float autoCalibrationDelay = 1f;      // Delay before auto-calibration (seconds)
    
    [Header("Debug")]
    public bool enableDebugLogs = true;            // Toggle for debug logs
    public float logInterval = 1.0f;               // How often to log (seconds)

    private LeapProvider provider;                 // Reference to the Leap Motion data provider
    private float lastLogTime = 0f;                // Timestamp for throttling logs
    private Vector3 calibrationPosition;           // Stored calibration position
    private bool calibrationAttempted = false;     // Whether auto-calibration has been attempted

    /// <summary>
    /// Initializes the component by finding the Leap Motion provider.
    /// </summary>
    void Start()
    {
        provider = FindObjectOfType<LeapProvider>();
        if (provider == null)
        {
            Debug.LogError("LeapMotionMovement: No LeapProvider found in scene!");
        }
        else
        {
            Debug.Log("LeapMotionMovement: Successfully found LeapProvider: " + provider.name);
            
            // Start auto-calibration timer if enabled
            if (autoCalibrate)
            {
                StartCoroutine(AutoCalibrateWithRetry());
            }
        }
    }

    /// <summary>
    /// Attempt auto-calibration with retry logic to ensure hands are detected
    /// </summary>
    private IEnumerator AutoCalibrateWithRetry()
    {
        yield return new WaitForSeconds(autoCalibrationDelay);
        
        // Try calibration for up to 5 seconds
        float attemptTime = 0f;
        bool calibrationSuccessful = false;
        
        while (attemptTime < 5.0f && !calibrationSuccessful)
        {
            calibrationSuccessful = AttemptCalibration();
            
            if (!calibrationSuccessful)
            {
                Debug.Log("LeapMotionMovement: Auto-calibration waiting for hands to be detected...");
                yield return new WaitForSeconds(0.5f);
                attemptTime += 0.5f;
            }
        }
        
        calibrationAttempted = true;
        
        if (!calibrationSuccessful)
        {
            Debug.LogWarning("LeapMotionMovement: Auto-calibration failed. Please press '" + calibrateKey + "' to manually calibrate.");
        }
    }
    
    /// <summary>
    /// Attempt a single calibration, returning true if successful
    /// </summary>
    private bool AttemptCalibration()
    {
        if (provider == null) return false;
        
        var frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return false;
        
        // If we have a hand, perform calibration
        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition;
        centerOffset = new Vector2(palmPosition.x, palmPosition.z);
        
        Debug.Log($"LeapMotionMovement: Auto-calibrated center position to {centerOffset}");
        return true;
    }

    /// <summary>
    /// Processes hand tracking data every frame and converts it to movement input.
    /// </summary>
    void Update()
    {
        // Check for calibration request
        if (Input.GetKeyDown(calibrateKey))
        {
            CalibrateCenter();
        }
        
        bool shouldLog = enableDebugLogs && Time.time - lastLogTime > logInterval;
        
        // Check if provider exists
        if (provider == null)
        {
            if (shouldLog)
            {
                Debug.LogWarning("LeapMotionMovement: LeapProvider is null");
                lastLogTime = Time.time;
            }
            return;
        }

        var frame = provider.CurrentFrame;
        // If no frame data or no hands detected, stop movement
        if (frame == null || frame.Hands.Count == 0)
        {
            // No hand detected, stop player movement
            playerScript.SendMessage("MovePressed", Vector2.zero);
            
            if (shouldLog)
            {
                Debug.Log("LeapMotionMovement: No hands detected, sending zero movement");
                lastLogTime = Time.time;
            }
            return;
        }

        // Get the first detected hand
        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition;

        // Extract horizontal position (x and z axes)
        Vector2 rawOffset = new Vector2(palmPosition.x, palmPosition.z);
        
        // Apply calibration if enabled
        Vector2 offset = useCalibration ? rawOffset - centerOffset : rawOffset;

        if (shouldLog)
        {
            Debug.Log($"LeapMotionMovement: Hand detected - Raw palm position: {palmPosition}, " +
                      $"Raw offset: {rawOffset}, Calibrated offset: {offset}, " +
                      $"Center offset: {centerOffset}, Magnitude: {offset.magnitude}");
            lastLogTime = Time.time;
        }

        // Apply dead zone - if hand is close to center position, don't move
        if (offset.magnitude < deadZoneRadius)
        {
            playerScript.SendMessage("MovePressed", Vector2.zero);
            if (shouldLog)
            {
                Debug.Log("LeapMotionMovement: Within dead zone, sending zero movement");
            }
        }
        else
        {
            // Normalize movement vector based on maximum offset and apply speed
            Vector2 clamped = Vector2.ClampMagnitude(offset / maxOffset, 1.0f);
            Vector2 moveVector = clamped * movementSpeed;
            
            playerScript.SendMessage("MovePressed", moveVector);
            
            if (shouldLog)
            {
                Debug.Log($"LeapMotionMovement: Sending movement - Clamped: {clamped}, Final vector: {moveVector}");
            }
        }
    }
    
    /// <summary>
    /// Calibrates the center position based on the current hand position
    /// </summary>
    public void CalibrateCenter()
    {
        if (provider == null) return;
        
        var frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0)
        {
            Debug.LogWarning("LeapMotionMovement: Cannot calibrate - no hands detected");
            return;
        }
        
        var hand = frame.Hands[0];
        Vector3 palmPosition = hand.PalmPosition;
        centerOffset = new Vector2(palmPosition.x, palmPosition.z);
        
        Debug.Log($"LeapMotionMovement: Calibrated center position to {centerOffset}");
    }
}
