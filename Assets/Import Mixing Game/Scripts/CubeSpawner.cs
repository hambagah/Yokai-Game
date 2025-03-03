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
            GameObject spawnedCube = Instantiate(cubePrefab, transform.position, transform.rotation);

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
