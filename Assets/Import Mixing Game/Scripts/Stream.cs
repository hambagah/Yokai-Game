/**
 * Stream.cs
 * 
 * Summary: Creates and manages a liquid stream visual effect.
 * Uses a line renderer and particle system to create a pouring liquid
 * that follows physics and splashes when hitting surfaces.
 */
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Stream : MonoBehaviour
{
    [Header("Stream Settings")]
    [Tooltip("How fast the stream moves in units per second")]
    [Range(0.5f, 5f)]
    public float streamSpeed = 1.75f;
    
    [Tooltip("Maximum distance the stream can reach")]
    [Range(0.5f, 10f)]
    public float maxStreamDistance = 2.0f;
    
    [Tooltip("Layers the stream can collide with")]
    public LayerMask collisionLayers = ~0; // Default to all layers
    
    [Tooltip("Radius of the collision detection ray")]
    [Range(0.01f, 0.5f)]
    public float streamRadius = 0.05f;

    // Component references
    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;

    // Internal state
    private Coroutine pourRoutine = null;
    private Coroutine particleRoutine = null;
    private Vector3 targetPosition = Vector3.zero;
    private bool isPouring = false;

    /**
     * Initialize components and check for required parts
     */
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Stream.cs: Missing LineRenderer component!");
            enabled = false;
            return;
        }

        splashParticle = GetComponentInChildren<ParticleSystem>();
        if (splashParticle == null)
        {
            Debug.LogWarning("Stream.cs: Missing Particle System on Stream object! No splash effects will be shown.");
        }
    }

    /**
     * Sets initial position of the stream points
     */
    private void Start()
    {
        InitializeLineRenderer();
    }

    /**
     * Initialize the line renderer with proper settings
     */
    private void InitializeLineRenderer()
    {
        if (lineRenderer != null)
        {
            // Ensure we have at least two points for the line
            if (lineRenderer.positionCount < 2)
                lineRenderer.positionCount = 2;
                
            // Initialize both points at the same position (no stream yet)
            MoveToPosition(0, transform.position);
            MoveToPosition(1, transform.position);
        }
    }

    /**
     * Starts the stream animation and particle effects
     * Called by PourDetector when pouring begins
     */
    public void Begin()
    {
        if (isPouring) return;
        
        isPouring = true;
        
        // Initialize the line renderer if needed
        InitializeLineRenderer();
        
        // Start the animation and particle routines
        if (splashParticle != null)
        {
            particleRoutine = StartCoroutine(UpdateParticle());
        }
        
        pourRoutine = StartCoroutine(BeginPour());
    }

    /**
     * Animates the stream continuously while pouring
     * Updates the end point based on physics raycasting
     */
    private IEnumerator BeginPour()
    {
        while (isPouring && gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            // Keep the start point at the origin
            MoveToPosition(0, transform.position);
            
            // Animate the end point to the target position
            AnimateToPosition(1, targetPosition);

            yield return null;
        }
    }

    /**
     * Stops the stream animation with a smooth transition
     * Called by PourDetector when pouring ends
     */
    public void End()
    {
        if (!isPouring) return;
        
        isPouring = false;
        
        // Stop the existing routines
        if (pourRoutine != null)
        {
            StopCoroutine(pourRoutine);
        }
        
        if (particleRoutine != null)
        {
            StopCoroutine(particleRoutine);
        }
        
        // Start the ending animation
        StartCoroutine(EndPour());
    }

    /**
     * Animates the stream ending by collapsing to the end point
     * Destroys the stream object when complete
     */
    private IEnumerator EndPour()
    {
        // Save the final target position
        Vector3 finalPosition = targetPosition;
        
        // Animate start point to target position
        while (!HasReachedPosition(0, finalPosition))
        {
            AnimateToPosition(0, finalPosition);
            AnimateToPosition(1, finalPosition);

            yield return null;
        }
        
        // Disable splash effects
        if (splashParticle != null && splashParticle.gameObject.activeSelf)
        {
            splashParticle.gameObject.SetActive(false);
        }
        
        // Small delay before destroying
        yield return new WaitForSeconds(0.1f);
        
        Destroy(gameObject);
    }

    /**
     * Uses physics raycast to find where the stream should end
     * Returns a position in world space
     */
    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        // Use SphereCast for better collision detection with small objects
        if (Physics.SphereCast(ray, streamRadius, out hit, maxStreamDistance, collisionLayers))
        {
            return hit.point;
        }
        
        // No collision, return point at maximum distance
        return ray.GetPoint(maxStreamDistance);
    }

    /**
     * Sets a line renderer point directly to a position
     * Used for instant position changes
     */
    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        if (lineRenderer != null && index < lineRenderer.positionCount)
        {
            lineRenderer.SetPosition(index, targetPosition);
        }
    }

    /**
     * Smoothly animates a line renderer point towards a target
     * Creates fluid motion for the stream
     */
    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        if (lineRenderer != null && index < lineRenderer.positionCount)
        {
            Vector3 currentPoint = lineRenderer.GetPosition(index);
            Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * streamSpeed);
            lineRenderer.SetPosition(index, newPosition);
        }
    }

    /**
     * Checks if a line renderer point has reached its target
     * Used to determine when animations are complete
     */
    private bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        if (lineRenderer != null && index < lineRenderer.positionCount)
        {
            Vector3 currentPosition = lineRenderer.GetPosition(index);
            return Vector3.Distance(currentPosition, targetPosition) < 0.01f;
        }
        return true;
    }

    /**
     * Updates the splash particle position and activation
     * Shows splash effect when stream hits a surface
     */
    private IEnumerator UpdateParticle()
    {
        if (splashParticle == null) yield break;
        
        while (isPouring && gameObject.activeSelf)
        {
            // Update particle position
            splashParticle.transform.position = targetPosition;

            // Only show particles when the stream is actually hitting something
            bool isHitting = HasReachedPosition(1, targetPosition);
            splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }
    }

    /**
     * Force destruct the stream object
     * Used for cleanup
     */
    private void OnDestroy()
    {
        if (pourRoutine != null)
        {
            StopCoroutine(pourRoutine);
        }
        
        if (particleRoutine != null)
        {
            StopCoroutine(particleRoutine);
        }
    }
}
