using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public string objectName;
        public GameObject prefab;
    }

    [Header("Spawn Settings")]
    public List<SpawnableObject> spawnableObjects;
    public int totalObjectsToSpawn = 10;
    public float spawnInterval = 0.5f;
    public Transform spawnPoint; // Single spawn point
    public float fallSpeed = 2.0f; // Speed at which objects fall

    [Header("Target Type")]
    public string targetObjectName; // e.g., "Apple"
    [HideInInspector] public int correctCount = 0;

    public delegate void SpawnComplete();
    public event SpawnComplete OnSpawnFinished;

    private int spawnedCount = 0;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void BeginSpawning()
    {
        // Clear any previous objects
        ClearObjects();
        
        spawnedCount = 0;
        correctCount = 0;
        StartCoroutine(SpawnRoutine());
    }

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
    
    public int GetTargetObjectCount()
    {
        return correctCount;
    }
    
    public void ClearObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}

// Simplified falling object class
public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 2.0f;
    public bool isTargetObject = false;
    
    private Rigidbody rb;
    
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
    
    void Update()
    {
        // Destroy if fallen too far below
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}
