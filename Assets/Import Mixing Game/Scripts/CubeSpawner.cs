/**
 * CubeSpawner.cs
 * 
 * Summary: Spawns ice cube prefabs at a specified position.
 * Creates new ice cubes for the player to use in the mixing game.
 */
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [Tooltip("Reference to the ice cube prefab to spawn")]
    public GameObject cubePrefab;
    
    [Tooltip("Parent transform to organize spawned cubes")]
    public Transform parentTransform;

    /**
     * Spawns a new ice cube at the spawner's position
     * Called by UI button press or other game events
     */
    public void SpawnCube()
    {
        if (cubePrefab != null)
        {
            // Create the ice cube at spawner position
            GameObject spawnedCube = Instantiate(cubePrefab, transform.position, transform.rotation);

            // Parent to specified transform for organization
            if (parentTransform != null)
            {
                spawnedCube.transform.SetParent(parentTransform);
            }
            else
            {
                Debug.LogWarning("CubeSpawner: Parent transform is NULL. The cube is spawned without a parent.");
            }
        }
        else
        {
            Debug.LogError("CubeSpawner: Cube prefab is not assigned!");
        }
    }
}
