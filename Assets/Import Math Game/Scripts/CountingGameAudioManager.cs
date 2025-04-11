using UnityEngine;

/// <summary>
/// Manages audio for the Counting Game, including object spawn sounds, 
/// countdown audio, and player turn audio cues.
/// </summary>
public class CountingGameAudioManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;         // For one-shot sound effects
    [SerializeField] private AudioSource objectSpawnSource; // For object spawn sounds

    [Header("Object Spawn Sounds")]
    [SerializeField] private AudioClip[] objectSpawnSounds; // Array of sounds to play when objects spawn
    [SerializeField] private bool randomizeSpawnPitch = true;
    [SerializeField] private Vector2 spawnPitchRange = new Vector2(0.9f, 1.1f);

    [Header("Game Flow Sounds")]
    [SerializeField] private AudioClip countdownSound;      // Sound for each countdown number
    [SerializeField] private AudioClip countdownFinishSound;// Sound when countdown completes
    [SerializeField] private AudioClip playerTurnSound;     // Sound when it's player's turn to sign

    [Header("Result Sounds")]
    [SerializeField] private AudioClip correctAnswerSound;  // Sound when player gives correct answer
    [SerializeField] private AudioClip wrongAnswerSound;    // Sound when player gives wrong answer

    #endregion

    #region Private Fields

    private CountingGameManager gameManager;
    private ObjectSpawnerManager objectSpawner;

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (objectSpawnSource == null)
        {
            objectSpawnSource = gameObject.AddComponent<AudioSource>();
            objectSpawnSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        gameManager = FindObjectOfType<CountingGameManager>();
        objectSpawner = FindObjectOfType<ObjectSpawnerManager>();

        if (gameManager == null)
        {
            Debug.LogError("CountingGameAudioManager: CountingGameManager not found in scene!");
            return;
        }

        if (objectSpawner == null)
        {
            Debug.LogError("CountingGameAudioManager: ObjectSpawnerManager not found in scene!");
            return;
        }

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Event Subscription

    private void SubscribeToEvents()
    {
        if (objectSpawner != null)
        {
            objectSpawner.OnObjectSpawned += PlayObjectSpawnSound;
            objectSpawner.OnSpawnFinished += PlayPlayerTurnSound;
        }

        if (gameManager != null)
        {
            gameManager.OnRoundCompleted += PlayResultSound;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (objectSpawner != null)
        {
            objectSpawner.OnObjectSpawned -= PlayObjectSpawnSound;
            objectSpawner.OnSpawnFinished -= PlayPlayerTurnSound;
        }

        if (gameManager != null)
        {
            gameManager.OnRoundCompleted -= PlayResultSound;
        }
    }

    #endregion

    #region Public Methods

    public void PlayCountdownNumberSound()
    {
        if (countdownSound != null)
        {
            sfxSource.PlayOneShot(countdownSound);
        }
    }

    public void PlayCountdownFinishSound()
    {
        if (countdownFinishSound != null)
        {
            sfxSource.PlayOneShot(countdownFinishSound);
        }
    }

    #endregion

    #region Sound Handlers

    private void PlayObjectSpawnSound(GameObject spawnedObject, bool isTarget)
    {
        if (objectSpawnSounds == null || objectSpawnSounds.Length == 0)
            return;

        AudioClip clip = objectSpawnSounds[Random.Range(0, objectSpawnSounds.Length)];
        if (clip != null)
        {
            if (randomizeSpawnPitch)
            {
                objectSpawnSource.pitch = Random.Range(spawnPitchRange.x, spawnPitchRange.y);
            }

            objectSpawnSource.PlayOneShot(clip);
        }
    }

    private void PlayPlayerTurnSound()
    {
        if (playerTurnSound != null)
        {
            sfxSource.PlayOneShot(playerTurnSound);
        }
    }

    public void PlayResultSound(bool correct)
    {
        if (correct && correctAnswerSound != null)
        {
            sfxSource.PlayOneShot(correctAnswerSound);
        }
        else if (!correct && wrongAnswerSound != null)
        {
            sfxSource.PlayOneShot(wrongAnswerSound);
        }
    }

    #endregion
}
