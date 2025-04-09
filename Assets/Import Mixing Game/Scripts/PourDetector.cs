/**
 * PourDetector.cs
 * 
 * Summary: Detects the tilt angle of a container and triggers pouring effects.
 * When the container is tilted beyond the threshold angle, a liquid stream is created.
 * This script acts as the core mechanic for pouring liquid ingredients into the mixing bowl.
 */
using UnityEngine;
using UnityEngine.Events;

public class PourDetector : MonoBehaviour
{
    [Header("Pour Configuration")]
    [Tooltip("Minimum angle in degrees to trigger pouring")]
    [Range(0, 90)]
    public int pourThreshold = 45;
    
    [Tooltip("Origin point where the stream starts")]
    public Transform origin = null;
    
    [Tooltip("Prefab for the liquid stream visual")]
    public GameObject StreamPrefab = null;

    [Tooltip("Maximum time in seconds the container can pour before being empty (0 = infinite)")]
    [Range(0, 30)]
    public float maxPourTime = 0f;

    [Tooltip("Time in seconds to refill the container (0 = instant)")]
    [Range(0, 5)]
    public float refillTime = 0f;

    [Header("Audio")]
    [Tooltip("Sound to play in a loop while pouring")]
    public AudioClip pourLoopSound;

    [Tooltip("Sound to play when container is empty")]
    public AudioClip emptySound;

    [Header("Events")]
    [Tooltip("Event triggered when pouring starts")]
    public UnityEvent onPourStart;
    
    [Tooltip("Event triggered when pouring ends")]
    public UnityEvent onPourEnd;

    // Internal state
    private bool isPouring = false;
    private Stream currentStream = null;
    private AudioSource audioSource = null;
    private float currentPourDuration = 0f;
    private float refillTimer = 0f;
    private bool isEmpty = false;

    /**
     * Initialize components
     */
    private void Awake()
    {
        // Get or add audio source component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (pourLoopSound != null || emptySound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f; // Make it 3D sound
            audioSource.volume = 0.7f;
            audioSource.loop = true; // Set to loop for the pour sound
        }
    }

    /**
     * Validate required components and settings
     */
    private void Start()
    {
        // Validate required components
        if (origin == null)
        {
            Debug.LogWarning("PourDetector: Origin transform is not assigned. Pouring will start from the object's position.");
            origin = transform;
        }

        if (StreamPrefab == null)
        {
            Debug.LogError("PourDetector: StreamPrefab is not assigned. Pouring visual effects will not work.");
        }
    }

    /**
     * Continuously checks pour angle and manages pouring state
     */
    private void Update()
    {
        bool pourCheck = !isEmpty && CalculatePourAngle() > pourThreshold;

        // Toggle pouring state when angle crosses threshold
        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if (isPouring)
                StartPour();
            else
                EndPour();
        }

        // If currently pouring and we have a max pour time
        if (isPouring && maxPourTime > 0)
        {
            currentPourDuration += Time.deltaTime;
            
            // Check if container is empty
            if (currentPourDuration >= maxPourTime && !isEmpty)
            {
                isEmpty = true;
                PlaySound(emptySound, false);
                EndPour();
                Debug.Log("Container is empty!");
            }
        }

        // Handle refilling if the container is empty and not pouring
        if (isEmpty && !isPouring)
        {
            HandleRefill();
        }
    }

    /**
     * Begins the pouring effect by creating a stream
     * Called when tilt angle exceeds threshold
     */
    private void StartPour()
    {
        if (isEmpty) return;

        Debug.Log("Pouring started");
        
        // Start looping pour sound
        if (audioSource != null && pourLoopSound != null)
        {
            audioSource.clip = pourLoopSound;
            audioSource.Play();
        }

        currentStream = CreateStream();
        if (currentStream != null)
        {
            currentStream.Begin();
            onPourStart?.Invoke();
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
        
        // Stop the looping pour sound
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
            onPourEnd?.Invoke();
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

    /**
     * Plays the specified audio clip
     * @param clip The audio clip to play
     * @param looping Whether the clip should loop
     */
    private void PlaySound(AudioClip clip, bool looping = false)
    {
        if (audioSource != null && clip != null)
        {
            // Stop any currently playing audio
            audioSource.Stop();
            
            // Set up the audio source
            audioSource.clip = clip;
            audioSource.loop = looping;
            
            // Play the sound
            audioSource.Play();
        }
    }

    /**
     * Handles the refill logic when the container is empty
     */
    private void HandleRefill()
    {
        if (refillTime <= 0)
        {
            // Instant refill
            isEmpty = false;
            currentPourDuration = 0f;
            Debug.Log("Container refilled instantly!");
            return;
        }

        // Gradual refill
        refillTimer += Time.deltaTime;
        if (refillTimer >= refillTime)
        {
            isEmpty = false;
            currentPourDuration = 0f;
            refillTimer = 0f;
            Debug.Log("Container refilled!");
        }
    }

    /**
     * Forces the container to be refilled immediately
     * Can be called by external scripts
     */
    public void ForceRefill()
    {
        isEmpty = false;
        currentPourDuration = 0f;
        refillTimer = 0f;
        Debug.Log("Container force-refilled!");
    }

    /**
     * Check if the container is currently pouring
     * Can be called by external scripts
     */
    public bool IsPouring()
    {
        return isPouring;
    }

    /**
     * Check if the container is empty
     * Can be called by external scripts
     */
    public bool IsEmpty()
    {
        return isEmpty;
    }

    /**
     * Gets the remaining pour amount as a percentage (0-1)
     * Can be called by external scripts for UI feedback
     */
    public float GetRemainingAmount()
    {
        if (maxPourTime <= 0) return 1f;
        return Mathf.Clamp01(1f - (currentPourDuration / maxPourTime));
    }
}
