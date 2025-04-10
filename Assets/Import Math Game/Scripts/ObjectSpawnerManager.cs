using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of objects for the counting game.
/// This script handles creating random objects, tracking how many target objects were spawned,
/// and provides events for when spawning completes.
/// </summary>
public class ObjectSpawnerManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public string objectName;   // Name identifier for the object
        public GameObject prefab;   // The prefab to spawn
    }

    [Header("Spawn Settings")]
    public List<SpawnableObject> spawnableObjects;   // Collection of different objects that can be spawned
    public int totalObjectsToSpawn = 10;             // Total number of objects to spawn in each round
    public float spawnInterval = 0.5f;               // Time between spawning each object
    public Transform spawnPoint;                     // Location where objects spawn
    public float fallSpeed = 2.0f;                   // Speed at which objects fall down

    [Header("Target Type")]
    public string targetObjectName;                  // The name of the object players need to count (e.g., "Apple")
    [HideInInspector] public int correctCount = 0;   // Counter for how many target objects were spawned

    // Event fired when all objects have been spawned
    public delegate void SpawnComplete();
    public event SpawnComplete OnSpawnFinished;

    private int spawnedCount = 0;                      // Tracks how many objects have been spawned so far
    private List<GameObject> spawnedObjects = new List<GameObject>();  // Keeps references to all spawned objects

    /// <summary>
    /// Start the spawning process - clears any previous objects and begins creating new ones
    /// </summary>
    public void BeginSpawning()
    {
        // Clear any previous objects
        ClearObjects();
        
        spawnedCount = 0;
        correctCount = 0;
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Coroutine that handles the spawning of objects over time
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        while (spawnedCount < totalObjectsToSpawn)
        {
            SpawnObject();
            spawnedCount++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // All objects have been spawned
        yield return new WaitForSeconds(spawnInterval * 2); // Give some extra time for visual clarity
        OnSpawnFinished?.Invoke();
    }

    /// <summary>
    /// Creates a single random object at the spawn point
    /// </summary>
    private void SpawnObject()
    {
        if (spawnableObjects.Count == 0) return;

        // Pick a random object
        int index = Random.Range(0, spawnableObjects.Count);
        var selected = spawnableObjects[index];

        // Use the single spawn point
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

        GameObject obj = Instantiate(selected.prefab, spawnPos, Quaternion.identity);
        spawnedObjects.Add(obj);
        
        // Track correct count
        if (selected.objectName == targetObjectName)
        {
            correctCount++;
        }
        
        // Add falling behavior
        FallingObject fallingObj = obj.AddComponent<FallingObject>();
        fallingObj.fallSpeed = fallSpeed;
        fallingObj.isTargetObject = (selected.objectName == targetObjectName);
    }
    
    /// <summary>
    /// Returns the number of target objects that were spawned
    /// </summary>
    public int GetTargetObjectCount()
    {
        return correctCount;
    }
    
    /// <summary>
    /// Removes all spawned objects from the scene
    /// </summary>
    public void ClearObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}

/// <summary>
/// Attached to spawned objects to handle their falling behavior and physics
/// </summary>
public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 2.0f;           // Speed at which the object falls
    public bool isTargetObject = false;      // Whether this is an object the player needs to count
    
    private Rigidbody rb;                    // Reference to the Rigidbody component
    
    /// <summary>
    /// Set up the physics behavior for the falling object
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Set up physics behavior
        rb.useGravity = true;
        rb.drag = 0.5f; // Add some air resistance for natural falling
        
        // Randomize initial rotation
        transform.rotation = Random.rotation;
    }
    
    /// <summary>
    /// Destroy the object if it falls too far below the scene
    /// </summary>
    void Update()
    {
        // Destroy if fallen too far below
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}
