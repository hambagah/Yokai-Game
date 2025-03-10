using System.Collections.Generic;
using UnityEngine;

public class IngredientResetter : MonoBehaviour
{
    public Transform movableObjectsParent; // Parent object that contains all moveable objects
    private Dictionary<Transform, (Vector3, Quaternion)> originalTransforms = new Dictionary<Transform, (Vector3, Quaternion)>();

    private void Start()
    {
        if (movableObjectsParent == null)
        {
            Debug.LogError("IngredientResetter: movableObjectsParent is NOT assigned!");
            return;
        }

        StoreOriginalPositions();
    }

    private void StoreOriginalPositions()
    {
        originalTransforms.Clear();

        foreach (Transform child in movableObjectsParent)
        {
            originalTransforms[child] = (child.position, child.rotation);
        }

        Debug.Log($"IngredientResetter: Stored positions for {originalTransforms.Count} ingredients.");
    }

    public void ResetIngredientPositions()
    {
        foreach (var kvp in originalTransforms)
        {
            if (kvp.Key != null)
            {
                kvp.Key.position = kvp.Value.Item1;
                kvp.Key.rotation = kvp.Value.Item2;
            }
            else
            {
                Debug.LogWarning($"IngredientResetter: Missing reference to an ingredient.");
            }
        }

        Debug.Log("IngredientResetter: All ingredients have been reset!");
    }

    public void RecalibrateOriginalPositions()
    {
        StoreOriginalPositions();
        Debug.Log("IngredientResetter: Original positions and rotations recalibrated.");
    }
}
