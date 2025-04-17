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
    public bool createIfMissing = true;
    
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
        
        // Check for and manage EventSystems
        EnsureActiveEventSystem();
    }
    
    private void OnEnable()
    {
        // Check for EventSystem whenever this object is enabled
        EnsureActiveEventSystem();
    }
    
    /**
     * Ensures there is exactly one active EventSystem in the scene
     */
    public void EnsureActiveEventSystem()
    {
        // Find all EventSystems in the scene
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>(true); // Include inactive objects
        
        if (eventSystems.Length > 0)
        {
            // Make sure the first one is active
            if (!eventSystems[0].gameObject.activeInHierarchy)
            {
                Debug.Log("Activating the first EventSystem found in the scene");
                eventSystems[0].gameObject.SetActive(true);
            }
            
            // Log if multiple systems are found
            if (eventSystems.Length > 1)
            {
                Debug.LogWarning($"Found {eventSystems.Length} EventSystems in the scene. Keeping only the first one active.");
                
                // Keep the first one active, disable the rest
                for (int i = 1; i < eventSystems.Length; i++)
                {
                    eventSystems[i].gameObject.SetActive(false);
                }
            }
        }
        // If no EventSystem exists and createIfMissing is true, create one
        else if (createIfMissing)
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
     * Static method to ensure there is exactly one active EventSystem
     * Can be called from other scripts if needed
     */
    public static void EnsureSingleEventSystem()
    {
        if (instance != null)
        {
            instance.EnsureActiveEventSystem();
        }
        else
        {
            // Create a temporary manager to handle the check
            GameObject tempManager = new GameObject("Temporary Event System Manager");
            EventSystemManager manager = tempManager.AddComponent<EventSystemManager>();
            manager.createIfMissing = true;
            manager.EnsureActiveEventSystem();
        }
    }
} 