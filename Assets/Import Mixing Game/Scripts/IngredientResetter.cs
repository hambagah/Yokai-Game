using System.Collections.Generic;
using UnityEngine;

public class IngredientResetter : MonoBehaviour
{
    public List<GameObject> ingredientObjects;  // List of ingredient objects to reset
    private List<Vector3> originalPositions = new List<Vector3>(); // List to store original positions
    private List<Quaternion> originalRotations = new List<Quaternion>(); // List to store original rotations

    public float resetThreshold = 10f;  // Distance threshold for resetting
    private void Start()
    {
        // Store the initial positions and rotations of the ingredients
        StoreOriginalPositions();
    }

private void Update()
{
    for (int i = 0; i < ingredientObjects.Count; i++)
    {
        if (ingredientObjects[i] != null)
        {
            float distance = Vector3.Distance(ingredientObjects[i].transform.position, originalPositions[i]);
            if (distance > resetThreshold)
            {
                ResetIngredientAtIndex(i);
            }
        }
    }
}

private void ResetIngredientAtIndex(int index)
{
    ingredientObjects[index].transform.position = originalPositions[index];
    ingredientObjects[index].transform.rotation = originalRotations[index];
    Debug.Log($"Ingredient {ingredientObjects[index].name} reset due to out-of-bounds movement.");
}


    private void StoreOriginalPositions()
    {
        originalPositions.Clear();  // Clear the list to avoid any old data
        originalRotations.Clear();  // Clear the list to avoid any old data

        foreach (var ingredient in ingredientObjects)
        {
            if (ingredient != null)
            {
                originalPositions.Add(ingredient.transform.position);
                originalRotations.Add(ingredient.transform.rotation);

                // Debug log to confirm the ingredient's position and rotation are stored
                Debug.Log($"Stored ingredient position: {ingredient.name} - Position: {ingredient.transform.position}, Rotation: {ingredient.transform.rotation}");
            }
            else
            {
                Debug.LogWarning("Null ingredient object found in the ingredient list.");
            }
        }
    }

    // Reset the positions and rotations of the ingredients to their original states
    public void ResetIngredientPositions()
    {
        if (ingredientObjects.Count != originalPositions.Count || ingredientObjects.Count != originalRotations.Count)
        {
            Debug.LogError("Mismatch in the number of ingredients or stored positions/rotations. Reset failed.");
            return;
        }

        for (int i = 0; i < ingredientObjects.Count; i++)
        {
            if (ingredientObjects[i] != null)
            {
                ingredientObjects[i].transform.position = originalPositions[i];
                ingredientObjects[i].transform.rotation = originalRotations[i];

                // Debug log to show that the ingredient's position and rotation are being reset
                Debug.Log($"Reset ingredient position: {ingredientObjects[i].name} - Position: {ingredientObjects[i].transform.position}, Rotation: {ingredientObjects[i].transform.rotation}");
            }
            else
            {
                Debug.LogWarning($"Ingredient {i} is null, skipping reset.");
            }
        }

        Debug.Log("All ingredients have been reset to their original positions!");
    }
}
