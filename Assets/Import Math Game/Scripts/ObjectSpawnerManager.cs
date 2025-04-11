using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Manages the spawning of objects for the counting game.
/// This script handles creating random objects, tracking how many target objects were spawned,
/// and provides events for when spawning completes.
/// </summary>
public class ObjectSpawnerManager : MonoBehaviour
{
    #region Nested Classes
    
    [System.Serializable]
    public class SpawnableObject
    {
        public string objectName;   // Name identifier for the object
        public GameObject prefab;   // The prefab to spawn
        public AudioClip spawnSound; // Sound effect for this object
        public Vector2 pitchRange = new Vector2(0.9f, 1.1f); // Pitch range for randomization
    }
    
    #endregion

    #region Serialized Fields
    
    [Header("Spawn Settings")]
    [SerializeField] private List<SpawnableObject> spawnableObjects = new List<SpawnableObject>();  // Collection of different objects that can be spawned
    [SerializeField] private int totalObjectsToSpawn = 10;             // Total number of objects to spawn in each round
    [SerializeField] private float spawnInterval = 0.5f;               // Time between spawning each object
    [SerializeField] private Transform spawnPoint;                     // Location where objects spawn
    [SerializeField] private float fallSpeed = 2.0f;                   // Speed at which objects fall down
    [SerializeField] private float postSpawnDelay = 1.0f;              // Extra delay after spawning all objects
    [SerializeField] private float destroyBelowY = -10f;               // Y position below which objects are destroyed
    
    [Header("Rotation Settings")]
    [SerializeField] private Vector2 rotationSpeedX = new Vector2(-90f, 90f);  // Range for X rotation speed
    [SerializeField] private Vector2 rotationSpeedY = new Vector2(-90f, 90f);  // Range for Y rotation speed
    [SerializeField] private Vector2 rotationSpeedZ = new Vector2(-45f, 45f);  // Range for Z rotation speed
    
    #endregion

    #region Public Properties
    
    [HideInInspector] public string targetObjectName;                  // The name of the object players need to count
    [HideInInspector] public int correctCount = 0;                     // Counter for how many target objects were spawned
    
    #endregion

    #region Events and Delegates
    
    // Event fired when all objects have been spawned
    public event Action OnSpawnFinished;
    
    // More detailed events
    public event Action<string> OnTargetObjectSelected;                // Fired when a new target object is selected
    public event Action<GameObject, bool> OnObjectSpawned;             // Fired when an object is spawned (with isTarget flag)
    
    #endregion

    #region Private Fields
    
    private int spawnedCount = 0;                                      // Tracks how many objects have been spawned so far
    private List<GameObject> spawnedObjects = new List<GameObject>();  // Keeps references to all spawned objects
    private Coroutine spawnCoroutine;                                  // Reference to the spawning coroutine
    
    #endregion

    #region Unity Lifecycle Methods
    
    private void OnDisable()
    {
        // Clean up any active coroutines
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Start the spawning process - clears any previous objects and begins creating new ones
    /// </summary>
    public void BeginSpawning()
    {
        // Clear any previous coroutine
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        
        // Clear any previous objects
        ClearObjects();
        
        // Pick a random target object for this round
        PickRandomTargetObject();
        
        // Reset counters
        spawnedCount = 0;
        correctCount = 0;
        
        // Start spawning
        spawnCoroutine = StartCoroutine(SpawnRoutine());
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
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        spawnedObjects.Clear();
    }
    
    /// <summary>
    /// Force-completes the spawning process (for testing or skipping)
    /// </summary>
    public void CompleteSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        
        OnSpawnFinished?.Invoke();
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Select a random object type to be the target for this round
    /// </summary>
    private void PickRandomTargetObject()
    {
        if (spawnableObjects == null || spawnableObjects.Count == 0)
        {
            Debug.LogWarning("No spawnable objects defined!");
            return;
        }
        
        int randomIndex = UnityEngine.Random.Range(0, spawnableObjects.Count);
        targetObjectName = spawnableObjects[randomIndex].objectName;
        
        // Trigger event for the newly selected target
        OnTargetObjectSelected?.Invoke(targetObjectName);
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

        // All objects have been spawned - add extra delay for visual clarity
        yield return new WaitForSeconds(postSpawnDelay);
        
        // Notify listeners that spawning is complete
        OnSpawnFinished?.Invoke();
        spawnCoroutine = null;
    }

    /// <summary>
    /// Creates a single random object at the spawn point
    /// </summary>
    private void SpawnObject()
    {
        if (spawnableObjects == null || spawnableObjects.Count == 0)
        {
            Debug.LogWarning("No spawnable objects defined!");
            return;
        }

        // Pick a random object
        int index = UnityEngine.Random.Range(0, spawnableObjects.Count);
        SpawnableObject selected = spawnableObjects[index];
        
        if (selected.prefab == null)
        {
            Debug.LogWarning($"Prefab for {selected.objectName} is null!");
            return;
        }

        // Use the spawn point or fall back to this transform
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

        // Instantiate the object
        GameObject obj = Instantiate(selected.prefab, spawnPos, Quaternion.identity);
        spawnedObjects.Add(obj);
        
        // Track if this is a target object
        bool isTarget = selected.objectName == targetObjectName;
        if (isTarget)
        {
            correctCount++;
        }
        
        // Add falling behavior
        FallingObject fallingObj = obj.AddComponent<FallingObject>();
        fallingObj.fallSpeed = fallSpeed;
        fallingObj.isTargetObject = isTarget;
        fallingObj.destroyBelowY = destroyBelowY;
        fallingObj.rotationSpeed = GenerateRandomRotation();
        
        // Play the spawn sound if it exists
        if (selected.spawnSound != null)
        {
            // Randomize pitch within the specified range
            float randomPitch = UnityEngine.Random.Range(selected.pitchRange.x, selected.pitchRange.y);
            
            // Create an AudioSource to play the sound
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            audioSource.clip = selected.spawnSound;
            audioSource.pitch = randomPitch;
            audioSource.Play();
            
            // Optionally, destroy the AudioSource after the sound has finished playing
            Destroy(audioSource, selected.spawnSound.length / randomPitch);
        }
        
        // Trigger event for the newly spawned object
        OnObjectSpawned?.Invoke(obj, isTarget);
    }
    
    /// <summary>
    /// Generate a random rotation vector based on configured ranges
    /// </summary>
    private Vector3 GenerateRandomRotation()
    {
        return new Vector3(
            UnityEngine.Random.Range(rotationSpeedX.x, rotationSpeedX.y),
            UnityEngine.Random.Range(rotationSpeedY.x, rotationSpeedY.y),
            UnityEngine.Random.Range(rotationSpeedZ.x, rotationSpeedZ.y)
        );
    }
    
    #endregion
}

/// <summary>
/// Attached to spawned objects to handle their falling behavior, physics, and rotation
/// </summary>
public class FallingObject : MonoBehaviour
{
    #region Public Fields
    
    public float fallSpeed = 2.0f;           // Speed at which the object falls
    public bool isTargetObject = false;      // Whether this is an object the player needs to count
    public Vector3 rotationSpeed;            // Speed and direction of object rotation
    public float destroyBelowY = -10f;       // Y position below which the object is destroyed
    
    #endregion

    #region Private Fields
    
    private Rigidbody rb;                    // Reference to the Rigidbody component
    
    #endregion

    #region Unity Lifecycle Methods
    
    /// <summary>
    /// Set up the physics behavior for the falling object
    /// </summary>
    void Start()
    {
        // Get or add rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure physics
        ConfigurePhysics();
        
        // Randomize initial rotation
        transform.rotation = UnityEngine.Random.rotation;
    }
    
    /// <summary>
    /// Update the object's rotation and check if it's fallen too far
    /// </summary>
    void Update()
    {
        // Rotate the object
        transform.Rotate(rotationSpeed * Time.deltaTime);
        
        // Destroy if fallen too far below
        if (transform.position.y < destroyBelowY)
        {
            Destroy(gameObject);
        }
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Configure the physics properties of the object
    /// </summary>
    private void ConfigurePhysics()
    {
        if (rb != null)
        {
            rb.useGravity = true;
            rb.drag = 0.5f;         // Add some air resistance for natural falling
            rb.angularDrag = 0.1f;  // Low angular drag for smoother rotation
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
    
    #endregion
}
