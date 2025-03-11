/**
 * PourDetector.cs
 * 
 * Summary: Detects the tilt angle of a container and triggers pouring effects.
 * When the container is tilted beyond the threshold angle, a liquid stream is created.
 */
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    [Tooltip("Minimum angle in degrees to trigger pouring")]
    public int pourThreshold = 45;
    
    [Tooltip("Origin point where the stream starts")]
    public Transform origin = null;
    
    [Tooltip("Prefab for the liquid stream visual")]
    public GameObject StreamPrefab = null;

    // Internal state
    private bool isPouring = false;
    private Stream currentStream = null;

    /**
     * Continuously checks pour angle and manages pouring state
     */
    private void Update()
    {
        bool pourCheck = CalculatePourAngle() > pourThreshold;

        // Toggle pouring state when angle crosses threshold
        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if (isPouring)
                StartPour();
            else
                EndPour();
        }
    }

    /**
     * Begins the pouring effect by creating a stream
     * Called when tilt angle exceeds threshold
     */
    private void StartPour()
    {
        Debug.Log("Pouring started");

        currentStream = CreateStream();
        if (currentStream != null)
        {
            currentStream.Begin();
        }
        else
        {
            Debug.LogError("Failed to create stream: StreamPrefab might be null or missing Stream component.");
        }
    }

    /**
     * Ends the pouring effect by terminating the stream
     * Called when tilt angle goes below threshold
     */
    private void EndPour()
    {
        Debug.Log("Pouring ended");
        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
        }
    }

    /**
     * Calculates the current tilt angle from vertical
     * Returns angle in degrees
     */
    private float CalculatePourAngle()
    {
        // Measures angle between container's up direction and world up
        return Vector3.Angle(transform.up, Vector3.up);
    }

    /**
     * Creates a new stream instance at the origin point
     * Returns the Stream component for control
     */
    private Stream CreateStream()
    {
        if (StreamPrefab == null || origin == null)
        {
            Debug.LogError("StreamPrefab or origin is not assigned.");
            return null;
        }

        GameObject streamObject = Instantiate(StreamPrefab, origin.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>();
    }
}
