/**
 * IngredientResetter.cs
 * 
 * Summary: Tracks and resets movable ingredients to their original positions.
 * Helps maintain gameplay flow by allowing players to reset the setup.
 */
using System.Collections.Generic;
using UnityEngine;

public class IngredientResetter : MonoBehaviour
{
    [Tooltip("Parent object containing all movable ingredient objects")]
    public Transform movableObjectsParent;
    
    // Dictionary to store original positions and rotations
    private Dictionary<Transform, (Vector3, Quaternion)> originalTransforms = new Dictionary<Transform, (Vector3, Quaternion)>();

    /**
     * Initialize by storing the original positions of all ingredients
     */
    private void Start()
    {
        if (movableObjectsParent == null)
        {
            Debug.LogError("IngredientResetter: movableObjectsParent is NOT assigned!");
            return;
        }

        StoreOriginalPositions();
    }

    /**
     * Records the original position and rotation of each child under the parent
     * Called at start and when recalibrating positions
     */
    private void StoreOriginalPositions()
    {
        originalTransforms.Clear();

        foreach (Transform child in movableObjectsParent)
        {
            originalTransforms[child] = (child.position, child.rotation);
        }

        Debug.Log($"IngredientResetter: Stored positions for {originalTransforms.Count} ingredients.");
    }

    /**
     * Resets all ingredients to their original positions and rotations
     * Called by UI button or other game events
     */
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

    /**
     * Updates the stored original positions with current positions
     * Useful after manual rearrangement or scene changes
     */
    public void RecalibrateOriginalPositions()
    {
        StoreOriginalPositions();
        Debug.Log("IngredientResetter: Original positions and rotations recalibrated.");
    }
}
