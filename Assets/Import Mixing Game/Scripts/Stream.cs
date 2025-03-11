/**
 * Stream.cs
 * 
 * Summary: Creates and manages a liquid stream visual effect.
 * Uses a line renderer and particle system to create a pouring liquid
 * that follows physics and splashes when hitting surfaces.
 */
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Stream : MonoBehaviour
{
    // Component references
    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;

    // Internal state
    private Coroutine pourRoutine = null;
    private Vector3 targetPosition = Vector3.zero;

    /**
     * Initialize components and check for required parts
     */
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();

        if (splashParticle == null)
        {
            Debug.LogError("Stream.cs: Missing Particle System on Stream object!");
        }
    }

    /**
     * Sets initial position of the stream points
     */
    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    /**
     * Starts the stream animation and particle effects
     * Called by PourDetector when pouring begins
     */
    public void Begin()
    {
        StartCoroutine(UpdateParticale());
        pourRoutine = StartCoroutine(BeginPour());
    }

    /**
     * Animates the stream continuously while pouring
     * Updates the end point based on physics raycasting
     */
    private IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToposition(1, targetPosition);

            yield return null;
        }
    }

    /**
     * Stops the stream animation with a smooth transition
     * Called by PourDetector when pouring ends
     */
    public void End()
    {
        StopCoroutine(pourRoutine);
        pourRoutine = StartCoroutine(EndPour());
    }

    /**
     * Animates the stream ending by collapsing to the end point
     * Destroys the stream object when complete
     */
    private IEnumerator EndPour()
    {
        while (!HasReachPosition(0, targetPosition))
        {
            AnimateToposition(0, targetPosition);
            AnimateToposition(1, targetPosition);

            yield return null;
        }
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

        Physics.Raycast(ray, out hit, 2.0f);
        Vector3 endPoint = hit.collider ? hit.point : ray.GetPoint(2.0f);

        return endPoint;
    }

    /**
     * Sets a line renderer point directly to a position
     * Used for instant position changes
     */
    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    /**
     * Smoothly animates a line renderer point towards a target
     * Creates fluid motion for the stream
     */
    private void AnimateToposition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * 1.75f);
        lineRenderer.SetPosition(index, newPosition);
    }

    /**
     * Checks if a line renderer point has reached its target
     * Used to determine when animations are complete
     */
    private bool HasReachPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

    /**
     * Updates the splash particle position and activation
     * Shows splash effect when stream hits a surface
     */
    private IEnumerator UpdateParticale()
    {
        while (gameObject.activeSelf)
        {
            splashParticle.gameObject.transform.position = targetPosition;

            bool isHitting = HasReachPosition(1, targetPosition);
            splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }
    }
}
