using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap;

/// <summary>
/// Manages the counting game where players count objects and show the answer with hand signs.
/// This script handles the game flow, pose detection, score tracking, and UI updates.
/// </summary>
public class CountingGameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private ObjectSpawnerManager objectSpawner;      // Manages spawning of countable objects
    [SerializeField] private HandPoseDetector handPoseDetector;       // Detects hand poses from Leap Motion
    [SerializeField] private HandPoseScriptableObject[] numberPoses;  // Array of hand poses for numbers 0-9
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI instructionText;         // Shows instructions to the player
    [SerializeField] private TextMeshProUGUI roundText;               // Shows current round info
    [SerializeField] private TextMeshProUGUI resultText;              // Shows results after each round
    [SerializeField] private GameObject resultPanel;                  // Panel showing results
    [SerializeField] private Button nextRoundButton;                  // Button to proceed to next round
    [SerializeField] private GameObject countdownPanel;               // Panel for countdown before round start
    [SerializeField] private TextMeshProUGUI countdownText;           // Text for countdown numbers
    
    [Header("Game Settings")]
    [SerializeField] private int totalRounds = 5;                     // Total number of rounds in the game
    [SerializeField] private int maxAttempts = 3;                     // Maximum number of attempts allowed
    [SerializeField] private float signHoldTime = 2.0f;               // Time to hold any sign before confirming it
    [SerializeField] private Image holdProgressBar;                   // Visual feedback for hold progress
    
    private int currentRound = 0;                                     // Current round number
    private int score = 0;                                            // Player's score (correct answers)
    private int remainingAttempts = 0;                                // Remaining attempts for current round
    private bool isWaitingForAnswer = false;                          // Whether waiting for player's answer
    
    private int lastDetectedNumber = -1;                              // Last number that was detected
    private float currentHoldTime = 0f;                               // Time player has held current sign
    private bool isHoldingSign = false;                               // Whether player is holding any sign
    
    private int expectedAnswer = 0;                                   // The correct count for this round
    private GameState currentState = GameState.Idle;                  // Current state of the game

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

    /// <summary>
    /// Set up event listeners when component is enabled
    /// </summary>
    private void OnEnable()
    {
        objectSpawner.OnSpawnFinished += HandleSpawnComplete;
        handPoseDetector.OnPoseDetected.AddListener(HandlePoseDetected);
        handPoseDetector.OnPoseLost.AddListener(HandlePoseLost);
        
        if (nextRoundButton != null)
            nextRoundButton.onClick.AddListener(StartNextRound);
    }

    /// <summary>
    /// Clean up event listeners when component is disabled
    /// </summary>
    private void OnDisable()
    {
        if (objectSpawner != null)
        {
            objectSpawner.OnSpawnFinished -= HandleSpawnComplete;
        }
            
        if (handPoseDetector != null)
        {
            handPoseDetector.OnPoseDetected.RemoveListener(HandlePoseDetected);
            handPoseDetector.OnPoseLost.RemoveListener(HandlePoseLost);
        }
        
        if (nextRoundButton != null)
            nextRoundButton.onClick.RemoveListener(StartNextRound);
    }

    /// <summary>
    /// Initialize the game and start first round
    /// </summary>
    private void Start()
    {
        // Configure HandPoseDetector
        handPoseDetector.SetPosesToDetect(new List<HandPoseScriptableObject>(numberPoses));
        
        resultPanel.SetActive(false);
        countdownPanel.SetActive(false);
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(false);
            
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
            holdProgressBar.fillAmount = currentHoldTime / signHoldTime;
        }

        // Check if sign has been held long enough
        if (currentHoldTime >= signHoldTime)
        {
            VerifySign(lastDetectedNumber);
        }
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
        countdownPanel.SetActive(true);
        
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        
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
        
        remainingAttempts = maxAttempts;
        roundText.text = $"Round {currentRound}/{totalRounds}";
        instructionText.text = "Get ready...";
        
        currentState = GameState.Spawning;
        objectSpawner.BeginSpawning();
        
        ResetHoldTimer();
        isWaitingForAnswer = false;
    }

    /// <summary>
    /// Handle when all objects have been spawned
    /// </summary>
    private void HandleSpawnComplete()
    {
        expectedAnswer = objectSpawner.GetTargetObjectCount();
        currentState = GameState.WaitingForPlayerAnswer;
        
        // Wait for player's answer
        UpdateInstructionForAttempts();
        isWaitingForAnswer = true;
        
        // Show hold progress bar
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(true);
    }

    /// <summary>
    /// Update the instruction text based on remaining attempts
    /// </summary>
    private void UpdateInstructionForAttempts()
    {
        instructionText.text = $"How many {objectSpawner.targetObjectName}s did you count? Show with your hand sign. " +
                               $"(Attempts left: {remainingAttempts})";
    }

    /// <summary>
    /// Handle when a pose is detected by the HandPoseDetector
    /// </summary>
    private void HandlePoseDetected()
    {
        if (currentState != GameState.WaitingForPlayerAnswer) return;

        HandPoseScriptableObject detectedPose = handPoseDetector.GetCurrentlyDetectedPose();
        if (detectedPose == null)
        {
            ResetHoldTimer();
            return;
        }

        // Find which number this pose represents
        int detectedNumber = -1;
        for (int i = 0; i < numberPoses.Length; i++)
        {
            if (detectedPose == numberPoses[i])
            {
                detectedNumber = i;
                break;
            }
        }

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
            
            // Update UI with the number being held
            instructionText.text = $"Hold sign '{detectedNumber}' to confirm...";
        }
    }

    /// <summary>
    /// Verify if the held sign is correct and process the result
    /// </summary>
    /// <param name="number">The number sign being shown</param>
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
            
            if (remainingAttempts <= 0)
            {
                // Out of attempts
                ShowResult(false, "Out of attempts!");
            }
            else
            {
                // Still have attempts left
                instructionText.text = $"That's {number}, not the correct count. " +
                                      $"Attempts left: {remainingAttempts}";
                ResetHoldTimer();
            }
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
            UpdateInstructionForAttempts();
        }
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
        }
    }

    /// <summary>
    /// Display result of the round to the player
    /// </summary>
    /// <param name="correct">Whether the answer was correct</param>
    /// <param name="message">Message to display</param>
    private void ShowResult(bool correct, string message)
    {
        currentState = GameState.ShowingResults;
        
        // Hide progress bar
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(false);
            
        resultPanel.SetActive(true);
        
        if (correct)
        {
            resultText.text = $"Correct! There were {expectedAnswer} {objectSpawner.targetObjectName}s.";
        }
        else
        {
            resultText.text = $"{message} There were {expectedAnswer} {objectSpawner.targetObjectName}s.";
        }
    }

    /// <summary>
    /// Start the next round of the game
    /// </summary>
    public void StartNextRound()
    {
        resultPanel.SetActive(false);
        objectSpawner.ClearObjects();
        StartCoroutine(StartRoundWithCountdown());
    }

    /// <summary>
    /// End the game and show final score
    /// </summary>
    private void EndGame()
    {
        currentState = GameState.ShowingResults;
        resultPanel.SetActive(true);
        resultText.text = $"Game Over!\nYour final score: {score}/{totalRounds}";
        
        // Option to restart game
        nextRoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";
    }
} 