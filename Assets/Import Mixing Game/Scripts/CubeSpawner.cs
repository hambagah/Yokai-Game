using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // Reference to the cube prefab to spawn
    public Transform parentTransform; // Parent transform for the spawned cubes

    // Function to spawn a cube at the spawner's position under a parent object
    public void SpawnCube()
    {
        if (cubePrefab != null)
        {
            // Instantiate the cube prefab at the spawner's position and rotation
            GameObject spawnedCube = Instantiate(cubePrefab, transform.position, transform.rotation);

            // Set the parent of the spawned cube, if a parent is specified
            if (parentTransform != null)
            {
                spawnedCube.transform.SetParent(parentTransform);
            }
        }
        else
        {
            Debug.LogWarning("Cube prefab is not assigned. Please assign a prefab in the inspector.");
        }
    }
}
