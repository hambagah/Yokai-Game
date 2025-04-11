using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap;
using System;

/// <summary>
/// Manages the counting game where players count objects and show the answer with hand signs.
/// This script handles the game flow, pose detection, score tracking, and UI updates.
/// </summary>
public class CountingGameManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Scene References")]
    [SerializeField] private ObjectSpawnerManager objectSpawner;      // Manages spawning of countable objects
    [SerializeField] private HandPoseDetector handPoseDetector;       // Detects hand poses from Leap Motion
    [SerializeField] private HandPoseScriptableObject[] numberPoses;  // Array of hand poses for numbers 0-9

    [Header("UI - Text Elements")]
    [SerializeField] private TextMeshProUGUI instructionText;         // Main instruction text
    [SerializeField] private TextMeshProUGUI statusText;              // Text showing what sign player is holding
    [SerializeField] private TextMeshProUGUI feedbackText;            // Text for feedback on incorrect answers
    [SerializeField] private TextMeshProUGUI roundText;               // Shows current round info
    [SerializeField] private TextMeshProUGUI resultText;              // Shows results after each round

    [Header("UI - Panels and Controls")]
    [SerializeField] private GameObject resultPanel;                  // Panel showing results
    [SerializeField] private Button nextRoundButton;                  // Button to proceed to next round
    [SerializeField] private GameObject countdownPanel;               // Panel for countdown before round start
    [SerializeField] private TextMeshProUGUI countdownText;           // Text for countdown numbers
    [SerializeField] private GameObject feedbackPanel;                // Panel containing the feedback text

    [Header("Progress Bar Settings")]
    [SerializeField] private Image holdProgressBar;                   // Visual feedback for hold progress
    [SerializeField] private Color startColor = Color.red;            // Color when progress is 0
    [SerializeField] private Color midColor = Color.yellow;           // Color when progress is 0.5
    [SerializeField] private Color endColor = Color.green;            // Color when progress is 1.0

    [Header("Game Settings")]
    [SerializeField] private int totalRounds = 5;                     // Total number of rounds in the game
    [SerializeField] private int maxAttempts = 3;                     // Maximum number of attempts allowed
    [SerializeField] private float signHoldTime = 2.0f;               // Time to hold any sign before confirming it
    [SerializeField] private float feedbackDisplayTime = 2.0f;        // How long to show the feedback panel
    [SerializeField] private float countdownInterval = 1.0f;          // Time between countdown numbers

    [Header("Object Spawner Settings")]
    [SerializeField] private bool overrideRigidbodySettings = true;

    [Header("Audio Manager")]
    [SerializeField] private CountingGameAudioManager audioManager;   // Manages game audio

    #endregion

    #region Private Fields

    private int currentRound = 0;                                     // Current round number
    private int score = 0;                                            // Player's score (correct answers)
    private int remainingAttempts = 0;                                // Remaining attempts for current round
    private bool isWaitingForAnswer = false;                          // Whether waiting for player's answer

    private int lastDetectedNumber = -1;                              // Last number that was detected
    private float currentHoldTime = 0f;                               // Time player has held current sign
    private bool isHoldingSign = false;                               // Whether player is holding any sign

    private int expectedAnswer = 0;                                   // The correct count for this round
    private GameState currentState = GameState.Idle;                  // Current state of the game
    private Coroutine feedbackCoroutine;                              // Reference to feedback display coroutine

    #endregion

    #region Events and Delegates

    // Add public events that other components could subscribe to
    public event Action<int> OnRoundStarted;                          // Fired when a new round starts
    public event Action<bool> OnRoundCompleted;                       // Fired when a round ends (success/failure)
    public event Action<int> OnGameCompleted;                         // Fired when the game is completed with final score

    #endregion

    #region Enums

    /// <summary>
    /// Possible states of the game
    /// </summary>
    private enum GameState
    {
        Idle,                       // Not currently running a round
        Spawning,                   // Objects are being spawned
        WaitingForPlayerAnswer,     // Waiting for player to show the correct sign
        ShowingResults              // Displaying results of the round
    }

    #endregion

    #region Unity Lifecycle Methods

    /// <summary>
    /// Set up event listeners when component is enabled
    /// </summary>
    private void OnEnable()
    {
        if (objectSpawner != null)
            objectSpawner.OnSpawnFinished += HandleSpawnComplete;

        if (handPoseDetector != null)
        {
            handPoseDetector.OnPoseDetected.AddListener(HandlePoseDetected);
            handPoseDetector.OnPoseLost.AddListener(HandlePoseLost);
        }

        if (nextRoundButton != null)
            nextRoundButton.onClick.AddListener(StartNextRound);
    }

    /// <summary>
    /// Clean up event listeners when component is disabled
    /// </summary>
    private void OnDisable()
    {
        if (objectSpawner != null)
            objectSpawner.OnSpawnFinished -= HandleSpawnComplete;

        if (handPoseDetector != null)
        {
            handPoseDetector.OnPoseDetected.RemoveListener(HandlePoseDetected);
            handPoseDetector.OnPoseLost.RemoveListener(HandlePoseLost);
        }

        if (nextRoundButton != null)
            nextRoundButton.onClick.RemoveListener(StartNextRound);

        // Clean up any active coroutines
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);
    }

    /// <summary>
    /// Initialize the game and start first round
    /// </summary>
    private void Start()
    {
        InitializeGame();
        StartGame();
    }

    /// <summary>
    /// Handle hold timer for poses in the answer phase
    /// </summary>
    private void Update()
    {
        if (currentState != GameState.WaitingForPlayerAnswer || !isHoldingSign)
            return;

        // Update hold timer for any sign
        currentHoldTime += Time.deltaTime;

        // Update progress bar
        if (holdProgressBar != null)
        {
            float fillAmount = Mathf.Clamp01(currentHoldTime / signHoldTime);
            holdProgressBar.fillAmount = fillAmount;

            // Update progress bar color based on fill amount
            UpdateProgressBarColor(fillAmount);
        }

        // Check if sign has been held long enough
        if (currentHoldTime >= signHoldTime)
        {
            VerifySign(lastDetectedNumber);
        }
    }

    #endregion

    #region Initialization and Game Flow

    /// <summary>
    /// Initialize UI elements and configuration
    /// </summary>
    private void InitializeGame()
    {
        // Configure HandPoseDetector
        if (handPoseDetector != null && numberPoses != null && numberPoses.Length > 0)
            handPoseDetector.SetPosesToDetect(new List<HandPoseScriptableObject>(numberPoses));

        // Initialize UI elements
        SetPanelStates(false);

        // Initialize progress bar
        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(false);
            holdProgressBar.color = startColor;
        }

        // Initialize text elements
        if (statusText != null)
            statusText.text = string.Empty;
        if (feedbackText != null)
            feedbackText.text = string.Empty;
    }

    /// <summary>
    /// Set active state of all panels at once
    /// </summary>
    private void SetPanelStates(bool active)
    {
        if (resultPanel != null)
            resultPanel.SetActive(active);
        if (countdownPanel != null)
            countdownPanel.SetActive(active);
        if (feedbackPanel != null)
            feedbackPanel.SetActive(active);
    }

    /// <summary>
    /// Start a new game by resetting score and beginning first round
    /// </summary>
    public void StartGame()
    {
        currentRound = 0;
        score = 0;
        currentState = GameState.Idle;
        StartCoroutine(StartRoundWithCountdown());
    }

    /// <summary>
    /// Start a round with a countdown animation
    /// </summary>
    private IEnumerator StartRoundWithCountdown()
    {
        // Show countdown UI
        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        // Countdown visuals (3, 2, 1)
        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = i.ToString();

            // Play sounds based on the countdown number
            if (audioManager != null)
            {
                if (i == 1)
                    audioManager.PlayCountdownFinishSound();
                else
                    audioManager.PlayCountdownNumberSound();
            }

            yield return new WaitForSeconds(countdownInterval);
        }

        // Hide countdown UI after finishing
        if (countdownPanel != null)
            countdownPanel.SetActive(false);

        StartRound();
    }



    /// <summary>
    /// Start a new round of the game
    /// </summary>
    private void StartRound()
    {
        currentRound++;
        if (currentRound > totalRounds)
        {
            EndGame();
            return;
        }

        // Reset round state
        remainingAttempts = maxAttempts;

        // Update UI
        if (roundText != null)
            roundText.text = $"Round {currentRound}/{totalRounds}";
        if (instructionText != null)
            instructionText.text = "Get ready...";
        if (statusText != null)
            statusText.text = string.Empty;

        // Hide feedback panel
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        // Start spawning objects
        currentState = GameState.Spawning;
        if (objectSpawner != null)
        {
            objectSpawner.BeginSpawning();
        }

        // Reset other state
        ResetHoldTimer();
        isWaitingForAnswer = false;

        // Trigger event
        OnRoundStarted?.Invoke(currentRound);
    }

    /// <summary>
    /// End the game and show final score
    /// </summary>
    private void EndGame()
    {
        currentState = GameState.ShowingResults;

        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = $"Game Over!\nYour final score: {score}/{totalRounds}";

        // Update button text for restart
        if (nextRoundButton != null && nextRoundButton.GetComponentInChildren<TextMeshProUGUI>() != null)
            nextRoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";

        // Trigger game completed event
        OnGameCompleted?.Invoke(score);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle when all objects have been spawned
    /// </summary>
    private void HandleSpawnComplete()
    {
        if (objectSpawner == null) return;

        expectedAnswer = objectSpawner.GetTargetObjectCount();
        currentState = GameState.WaitingForPlayerAnswer;

        // Set the question text
        if (instructionText != null)
            instructionText.text = $"How many {objectSpawner.targetObjectName}s did you count?";

        // Set status text
        UpdateStatusText();

        isWaitingForAnswer = true;

        // Show hold progress bar
        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(true);
            holdProgressBar.color = startColor;
            holdProgressBar.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Handle when a pose is detected by the HandPoseDetector
    /// </summary>
    private void HandlePoseDetected()
    {
        if (currentState != GameState.WaitingForPlayerAnswer || handPoseDetector == null)
            return;

        HandPoseScriptableObject detectedPose = handPoseDetector.GetCurrentlyDetectedPose();
        if (detectedPose == null)
        {
            ResetHoldTimer();
            return;
        }

        // Find which number this pose represents
        int detectedNumber = GetNumberFromPose(detectedPose);
        if (detectedNumber == -1)
        {
            ResetHoldTimer();
            return;
        }

        // If we detect a different sign than before, reset the timer
        if (lastDetectedNumber != detectedNumber)
        {
            ResetHoldTimer();
            lastDetectedNumber = detectedNumber;
            isHoldingSign = true;

            // Update status text with the number being held
            if (statusText != null)
                statusText.text = $"Hold sign '{detectedNumber}' to confirm...";
        }
    }

    /// <summary>
    /// Handle when a pose is lost by the HandPoseDetector
    /// </summary>
    private void HandlePoseLost()
    {
        ResetHoldTimer();

        if (currentState == GameState.WaitingForPlayerAnswer)
        {
            UpdateStatusText();
        }
    }

    /// <summary>
    /// Start the next round of the game
    /// </summary>
    public void StartNextRound()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (objectSpawner != null)
            objectSpawner.ClearObjects();

        StartCoroutine(StartRoundWithCountdown());
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Get the number associated with the detected pose
    /// </summary>
    private int GetNumberFromPose(HandPoseScriptableObject pose)
    {
        if (numberPoses == null) return -1;

        for (int i = 0; i < numberPoses.Length; i++)
        {
            if (pose == numberPoses[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Update the status text based on remaining attempts
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText != null)
            statusText.text = $"Show the answer with your hand sign.\n(Attempts left: {remainingAttempts})";
    }

    /// <summary>
    /// Show feedback when player makes an incorrect answer
    /// </summary>
    private void ShowFeedback(string message)
    {
        if (feedbackText == null || feedbackPanel == null) return;

        // Stop any existing feedback coroutine
        if (feedbackCoroutine != null)
            StopCoroutine(feedbackCoroutine);

        // Start new feedback display
        feedbackCoroutine = StartCoroutine(ShowFeedbackCoroutine(message));
    }

    /// <summary>
    /// Coroutine to show feedback temporarily
    /// </summary>
    private IEnumerator ShowFeedbackCoroutine(string message)
    {
        feedbackText.text = message;
        feedbackPanel.SetActive(true);

        yield return new WaitForSeconds(feedbackDisplayTime);

        feedbackPanel.SetActive(false);
        feedbackCoroutine = null;
    }

    /// <summary>
    /// Update the progress bar color based on fill amount
    /// </summary>
    private void UpdateProgressBarColor(float fillAmount)
    {
        if (holdProgressBar == null) return;

        Color newColor;

        if (fillAmount <= 0.5f)
        {
            // Lerp from start color to mid color (0-0.5)
            newColor = Color.Lerp(startColor, midColor, fillAmount * 2f);
        }
        else
        {
            // Lerp from mid color to end color (0.5-1)
            newColor = Color.Lerp(midColor, endColor, (fillAmount - 0.5f) * 2f);
        }

        holdProgressBar.color = newColor;
    }

    /// <summary>
    /// Reset the hold timer when pose is lost or changed
    /// </summary>
    private void ResetHoldTimer()
    {
        currentHoldTime = 0f;
        isHoldingSign = false;
        lastDetectedNumber = -1;

        if (holdProgressBar != null)
        {
            holdProgressBar.fillAmount = 0f;
            holdProgressBar.color = startColor;
        }
    }

    #endregion

    #region Game Logic

    /// <summary>
    /// Verify if the held sign is correct and process the result
    /// </summary>
    private void VerifySign(int number)
    {
        isHoldingSign = false;

        // Check if the sign matches the expected answer
        if (number == expectedAnswer)
        {
            // Correct answer
            score++;
            ShowResult(true, "Correct!");
        }
        else
        {
            // Incorrect answer - reduce attempts
            remainingAttempts--;
            audioManager?.PlayResultSound(false); //play incorrect audio


            if (remainingAttempts <= 0)
            {
                // Out of attempts
                ShowResult(false, "Out of attempts!");
            }
            else
            {
                // Still have attempts left
                ShowFeedback($"That's {number}, not the correct count.\nAttempts left: {remainingAttempts}");
                UpdateStatusText(); // Reset the status text
                ResetHoldTimer();
            }
        }
    }

    /// <summary>
    /// Display result of the round to the player
    /// </summary>
    private void ShowResult(bool correct, string message)
    {
        currentState = GameState.ShowingResults;

        // Hide UI elements
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(false);
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        // Show result
        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
        {
            if (correct)
            {
                resultText.text = $"Correct!\nThere were {expectedAnswer} {objectSpawner.targetObjectName}s.";
            }
            else
            {
                resultText.text = $"{message}\nThere were {expectedAnswer} {objectSpawner.targetObjectName}s.";
            }
        }

        // Trigger event
        OnRoundCompleted?.Invoke(correct);
    }

    #endregion

    /// <summary>
    /// Event handler for round started event
    /// </summary>
    private void PlayCountdownSounds(int roundNumber)
    {
        // We no longer need this since countdown sounds are
        // played directly from CountingGameManager
        // StartCoroutine(CountdownSoundsCoroutine());
    }
}