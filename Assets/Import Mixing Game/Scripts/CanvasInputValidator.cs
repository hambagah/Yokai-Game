using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/**
 * CanvasInputValidator.cs
 * 
 * Summary: Checks and ensures that Canvas and EventSystem components are 
 * properly configured for UI interaction at runtime. Fixes common issues
 * that could prevent buttons from working properly.
 */
public class CanvasInputValidator : MonoBehaviour
{
    [Tooltip("Whether to automatically fix issues that are found")]
    public bool autoFixIssues = true;
    
    [Tooltip("Whether to check all canvases or just the one this component is attached to")]
    public bool checkAllCanvases = true;
    
    [Tooltip("Whether to check on scene start")]
    public bool checkOnStart = true;
    
    private void Start()
    {
        if (checkOnStart)
        {
            ValidateCanvasSetup();
        }
    }
    
    /**
     * Runs a validation check on all Canvas components in the scene
     * to ensure they're properly set up for input interactions
     */
    public void ValidateCanvasSetup()
    {
        // First make sure we have an EventSystem
        ValidateEventSystem();
        
        // Get all canvases to check
        Canvas[] canvasesToCheck;
        if (checkAllCanvases)
        {
            canvasesToCheck = FindObjectsOfType<Canvas>();
        }
        else
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("CanvasInputValidator: No Canvas component found on this GameObject!");
                return;
            }
            canvasesToCheck = new Canvas[] { canvas };
        }
        
        // Check each canvas
        foreach (Canvas canvas in canvasesToCheck)
        {
            ValidateSingleCanvas(canvas);
        }
    }
    
    /**
     * Makes sure there's at least one active EventSystem in the scene
     */
    private void ValidateEventSystem()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>();
        if (systems.Length == 0)
        {
            Debug.LogWarning("CanvasInputValidator: No EventSystem found in the scene.");
            
            if (autoFixIssues)
            {
                Debug.Log("CanvasInputValidator: Creating a new EventSystem.");
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }
        }
        else
        {
            bool anyActive = false;
            foreach (EventSystem system in systems)
            {
                if (system.gameObject.activeInHierarchy && system.enabled)
                {
                    anyActive = true;
                    break;
                }
            }
            
            if (!anyActive && autoFixIssues)
            {
                Debug.LogWarning("CanvasInputValidator: No active EventSystem found. Activating one...");
                systems[0].gameObject.SetActive(true);
                systems[0].enabled = true;
            }
        }
    }
    
    /**
     * Validates a single Canvas component to ensure it's properly set up for input
     */
    private void ValidateSingleCanvas(Canvas canvas)
    {
        if (canvas == null) return;
        
        // Check if the canvas has a GraphicRaycaster
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogWarning($"CanvasInputValidator: Canvas '{canvas.gameObject.name}' is missing a GraphicRaycaster component.");
            
            if (autoFixIssues)
            {
                Debug.Log($"CanvasInputValidator: Adding GraphicRaycaster to '{canvas.gameObject.name}'.");
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        else if (!raycaster.enabled)
        {
            Debug.LogWarning($"CanvasInputValidator: Canvas '{canvas.gameObject.name}' has a disabled GraphicRaycaster.");
            
            if (autoFixIssues)
            {
                Debug.Log($"CanvasInputValidator: Enabling GraphicRaycaster on '{canvas.gameObject.name}'.");
                raycaster.enabled = true;
            }
        }
        
        // Check if the canvas is in a valid render mode for input
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Debug.Log($"CanvasInputValidator: Canvas '{canvas.gameObject.name}' is in World Space mode. " +
                     "This requires a camera to be properly assigned for input.");
            
            if (canvas.worldCamera == null)
            {
                Debug.LogWarning($"CanvasInputValidator: World Space Canvas '{canvas.gameObject.name}' has no camera assigned.");
                
                if (autoFixIssues)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        Debug.Log($"CanvasInputValidator: Assigning main camera to World Space Canvas '{canvas.gameObject.name}'.");
                        canvas.worldCamera = mainCamera;
                    }
                    else
                    {
                        Debug.LogError("CanvasInputValidator: Cannot find a main camera to assign to the World Space Canvas.");
                    }
                }
            }
        }
        
        // Check all Button components under this canvas for proper setup
        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            ValidateButton(button);
        }
    }
    
    /**
     * Validates a Button component to ensure it's properly set up
     */
    private void ValidateButton(Button button)
    {
        if (button == null) return;
        
        // Check if the button has a target graphic
        if (button.targetGraphic == null)
        {
            Debug.LogWarning($"CanvasInputValidator: Button '{button.gameObject.name}' has no target graphic assigned.");
            
            if (autoFixIssues)
            {
                // Try to find an Image component on the same GameObject
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    Debug.Log($"CanvasInputValidator: Assigning Image component as target graphic for Button '{button.gameObject.name}'.");
                    button.targetGraphic = image;
                }
            }
        }
        
        // Check if the button's graphic is raycast target
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null && !buttonImage.raycastTarget)
        {
            Debug.LogWarning($"CanvasInputValidator: Button '{button.gameObject.name}' has an Image with raycastTarget=false, which will prevent clicks.");
            
            if (autoFixIssues)
            {
                Debug.Log($"CanvasInputValidator: Enabling raycastTarget for Button '{button.gameObject.name}'.");
                buttonImage.raycastTarget = true;
            }
        }
        
        // Check if the button has any onClick listeners
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            Debug.Log($"CanvasInputValidator: Button '{button.gameObject.name}' has no onClick event listeners.");
        }
    }
} 