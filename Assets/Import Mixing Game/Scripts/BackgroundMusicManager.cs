using UnityEngine;

/**
 * BackgroundMusicManager.cs
 * 
 * Summary: Manages background music for the mixing game scene.
 * Features:
 * - Plays background music in loop
 * - Optional persistence between scenes
 * - Volume control
 * - Fade in/out effects
 */
public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Audio Configuration")]
    [Tooltip("The main background music audio clip")]
    public AudioClip backgroundMusic;

    [Tooltip("Volume of the background music (0-1)")]
    [Range(0, 1)]
    public float musicVolume = 0.5f;

    [Tooltip("Whether to persist between scene loads")]
    public bool dontDestroyOnLoad = true;

    [Tooltip("Time to fade in music at start (seconds)")]
    [Range(0, 5)]
    public float fadeInTime = 1.0f;

    private AudioSource audioSource;
    private static BackgroundMusicManager instance;
    private float targetVolume;

    /**
     * Initialize the audio source and enforce singleton pattern
     */
    private void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure audio source
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0; // Start at zero for fade-in
        targetVolume = musicVolume;

        // Start playing if we have a clip assigned
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
        else
        {
            Debug.LogWarning("BackgroundMusicManager: No background music clip assigned!");
        }
    }

    /**
     * Handle fade-in effect
     */
    private void Update()
    {
        // Fade in if needed
        if (audioSource.volume < targetVolume && audioSource.isPlaying)
        {
            float newVolume = audioSource.volume;
            if (fadeInTime > 0)
            {
                newVolume += Time.deltaTime / fadeInTime * targetVolume;
            }
            else
            {
                newVolume = targetVolume; // Immediate
            }
            
            audioSource.volume = Mathf.Min(newVolume, targetVolume);
        }
    }

    /**
     * Start playing the background music with fade-in
     */
    public void PlayBackgroundMusic()
    {
        if (audioSource == null || backgroundMusic == null) return;

        audioSource.clip = backgroundMusic;
        audioSource.volume = 0; // Start silent for fade-in
        audioSource.Play();
        
        Debug.Log("Background music started playing");
    }

    /**
     * Change the volume of the background music
     */
    public void SetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
        musicVolume = targetVolume;
        
        // If we're not fading in, set the volume immediately
        if (!audioSource.isPlaying || fadeInTime <= 0)
        {
            audioSource.volume = targetVolume;
        }
    }

    /**
     * Pause the background music
     */
    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    /**
     * Resume the background music if paused
     */
    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.UnPause();
        }
    }

    /**
     * Stop the background music
     */
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /**
     * Access the singleton instance
     */
    public static BackgroundMusicManager Instance
    {
        get { return instance; }
    }
} 