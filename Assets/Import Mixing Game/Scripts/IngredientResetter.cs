using System.Collections.Generic;
using UnityEngine;

public class IngredientResetter : MonoBehaviour
{
    public Transform movableObjectsParent; // Parent object that contains all moveable objects
    private List<Vector3> originalPositions = new List<Vector3>(); // Stores original positions
    private List<Quaternion> originalRotations = new List<Quaternion>(); // Stores original rotations
    private List<Transform> ingredientObjects = new List<Transform>(); // List of ingredients

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
        originalPositions.Clear();
        originalRotations.Clear();
        ingredientObjects.Clear();

        // Get all child objects inside the parent
        foreach (Transform child in movableObjectsParent)
        {
            ingredientObjects.Add(child);
            originalPositions.Add(child.position);
            originalRotations.Add(child.rotation);
        }

        Debug.Log($"IngredientResetter: Stored positions for {ingredientObjects.Count} ingredients.");
    }

    public void ResetIngredientPositions()
    {
        if (ingredientObjects.Count != originalPositions.Count || ingredientObjects.Count != originalRotations.Count)
        {
            Debug.LogError("IngredientResetter: Mismatch in stored positions/rotations. Reset failed.");
            return;
        }

        for (int i = 0; i < ingredientObjects.Count; i++)
        {
            if (ingredientObjects[i] != null)
            {
                ingredientObjects[i].position = originalPositions[i];
                ingredientObjects[i].rotation = originalRotations[i];
            }
            else
            {
                Debug.LogWarning($"IngredientResetter: Ingredient {i} is null, skipping reset.");
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
