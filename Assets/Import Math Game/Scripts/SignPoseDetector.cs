using UnityEngine;
using Leap;

public class SignPoseDetector : MonoBehaviour
{
    private HandPoseDetector poseDetector;
    public bool HasBeenDetected { get; private set; }

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

    private void HandlePoseDetected()
    {
        HasBeenDetected = true;
    }

    private void HandlePoseLost()
    {
        HasBeenDetected = false;
    }

    public void Reset()
    {
        HasBeenDetected = false;
    }

    public void SetActive(bool active)
    {
        poseDetector.enabled = active;
        if (active)
        {
            Reset();
        }
    }

    private void OnDestroy()
    {
        if (poseDetector != null)
        {
            poseDetector.OnPoseDetected.RemoveListener(HandlePoseDetected);
            poseDetector.OnPoseLost.RemoveListener(HandlePoseLost);
        }
    }
} 