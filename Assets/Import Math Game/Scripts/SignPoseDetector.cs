using UnityEngine;
using Leap;

/// <summary>
/// A wrapper around the HandPoseDetector that simplifies pose detection and tracking.
/// This script provides simple interfaces to check if a pose has been detected
/// and to enable/disable the detector as needed.
/// </summary>
public class SignPoseDetector : MonoBehaviour
{
    private HandPoseDetector poseDetector;                  // Reference to the HandPoseDetector component
    public bool HasBeenDetected { get; private set; }       // Flag indicating if the pose has been detected

    /// <summary>
    /// Initialize and set up event listeners for pose detection
    /// </summary>
    private void Awake()
    {
        poseDetector = GetComponent<HandPoseDetector>();
        if (poseDetector == null)
        {
            Debug.LogError("No HandPoseDetector component found!");
            return;
        }

        // Subscribe to the correct events for HandPoseDetector
        poseDetector.OnPoseDetected.AddListener(HandlePoseDetected);
        poseDetector.OnPoseLost.AddListener(HandlePoseLost);
    }

    /// <summary>
    /// Called when the HandPoseDetector detects a pose
    /// </summary>
    private void HandlePoseDetected()
    {
        HasBeenDetected = true;
    }

    /// <summary>
    /// Called when the HandPoseDetector loses a previously detected pose
    /// </summary>
    private void HandlePoseLost()
    {
        HasBeenDetected = false;
    }

    /// <summary>
    /// Reset the detection state
    /// </summary>
    public void Reset()
    {
        HasBeenDetected = false;
    }

    /// <summary>
    /// Enable or disable the pose detector
    /// </summary>
    /// <param name="active">Whether the detector should be active</param>
    public void SetActive(bool active)
    {
        poseDetector.enabled = active;
        if (active)
        {
            Reset();
        }
    }

    /// <summary>
    /// Clean up event listeners when the component is destroyed
    /// </summary>
    private void OnDestroy()
    {
        if (poseDetector != null)
        {
            poseDetector.OnPoseDetected.RemoveListener(HandlePoseDetected);
            poseDetector.OnPoseLost.RemoveListener(HandlePoseLost);
        }
    }
} 