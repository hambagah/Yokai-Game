using UnityEngine;
using UnityEngine.EventSystems;

/**
 * EventSystemManager.cs
 * 
 * Summary: Ensures there is only one EventSystem active in the scene at any time.
 * When the scene loads, this script checks for multiple EventSystems and disables duplicates.
 * If no EventSystem exists, it can optionally create one.
 */
public class EventSystemManager : MonoBehaviour
{
    [Tooltip("Whether this EventSystem should be preserved when a new scene loads")]
    public bool dontDestroyOnLoad = true;
    
    [Tooltip("Whether to create an EventSystem if none exists in the scene")]
    public bool createIfMissing = false;
    
    // Static reference to maintain singleton pattern
    private static EventSystemManager instance;
    
    private void Awake()
    {
        // If this is the first instance, make it the singleton
        if (instance == null)
        {
            instance = this;
            
            // Apply DontDestroyOnLoad if enabled
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        // If another instance exists, destroy this one
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Check for and remove duplicate EventSystems
        CheckForDuplicateEventSystems();
    }
    
    private void OnEnable()
    {
        // Check for duplicates whenever this object is enabled
        CheckForDuplicateEventSystems();
    }
    
    private void Start()
    {
        // Additional check for duplicates on Start
        CheckForDuplicateEventSystems();
    }
    
    /**
     * Checks for duplicate EventSystem components in the scene and removes them
     */
    public void CheckForDuplicateEventSystems()
    {
        // Find all EventSystems in the scene
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        
        // Log if multiple systems are found
        if (eventSystems.Length > 1)
        {
            Debug.LogWarning($"Found {eventSystems.Length} EventSystems in the scene. Keeping only one.");
            
            // Keep the first one, disable the rest
            for (int i = 1; i < eventSystems.Length; i++)
            {
                EventSystem duplicate = eventSystems[i];
                
                // Keep our own EventSystem if it's part of our GameObject
                if (duplicate.gameObject == this.gameObject)
                {
                    // Disable the other one instead
                    eventSystems[0].gameObject.SetActive(false);
                    Debug.Log($"Disabled EventSystem on {eventSystems[0].gameObject.name}");
                    break;
                }
                else
                {
                    // Disable the duplicate
                    duplicate.gameObject.SetActive(false);
                    Debug.Log($"Disabled EventSystem on {duplicate.gameObject.name}");
                }
            }
        }
        // If no EventSystem exists and createIfMissing is true, create one
        else if (eventSystems.Length == 0 && createIfMissing)
        {
            CreateEventSystem();
        }
    }
    
    /**
     * Creates a new EventSystem if none exists in the scene
     */
    private void CreateEventSystem()
    {
        // Create a new GameObject for the EventSystem
        GameObject eventSystemObject = new GameObject("EventSystem");
        
        // Add required components
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
        
        Debug.Log("Created new EventSystem since none was found in the scene.");
    }
    
    /**
     * Static method to ensure only one EventSystem exists
     * Can be called from other scripts if needed
     */
    public static void EnsureSingleEventSystem()
    {
        if (instance != null)
        {
            instance.CheckForDuplicateEventSystems();
        }
        else
        {
            // Create a temporary manager to handle the check
            GameObject tempManager = new GameObject("Temporary Event System Manager");
            EventSystemManager manager = tempManager.AddComponent<EventSystemManager>();
            manager.createIfMissing = true;
            manager.CheckForDuplicateEventSystems();
            
            // The manager will clean itself up if another instance already exists
        }
    }
} 