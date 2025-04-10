using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap;
using System;

/// <summary>
/// Manages the learning phase of the game where players learn to make hand signs for numbers.
/// This script guides players through learning each number sign sequentially, requiring
/// players to hold each sign for a minimum time before advancing to the next number.
/// </summary>
public class SignLearningManager : MonoBehaviour
{
    [Header("Pose Detection")]
    [SerializeField] private HandPoseDetector handPoseDetector;       // Main detector for hand poses
    [SerializeField] private HandPoseScriptableObject[] numberPoses;  // Array of 10 poses (0-9)
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;         // Text showing current instructions
    [SerializeField] private Image handSignImage;                     // Image showing the sign to perform
    [SerializeField] private GameObject successIcon;                  // Icon that appears on successful detection
    [SerializeField] private GameObject completionPanel;              // Panel shown when all signs are learned
    [SerializeField] private float successIconDisplayTime = 1.0f;     // How long to show the success icon
    
    [Header("Optional")]
    [SerializeField] private Sprite[] numberHandSignSprites;          // Visual references for each hand sign

    [Header("Gesture Timing")]
    [SerializeField] private float requiredHoldTime = 2.0f;           // Time in seconds to hold the gesture
    [SerializeField] private Image holdProgressBar;                   // Visual feedback of hold progress
    
    private int currentNumberIndex = 0;                               // Current number being learned (0-9)
    private bool isWaitingForPose = false;                            // Whether waiting for player input
    private Coroutine successCoroutine;                               // Reference to success animation coroutine
    private float currentHoldTime = 0f;                               // How long the current pose has been held
    private bool isHoldingCorrectPose = false;                        // Whether the player is making the right sign

    /// <summary>
    /// Set up event listeners when component is enabled
    /// </summary>
    private void OnEnable()
    {
        if (handPoseDetector == null)
        {
            Debug.LogError("HandPoseDetector reference is missing!");
            return;
        }

        handPoseDetector.OnPoseDetected.AddListener(HandlePoseDetected);
        handPoseDetector.OnPoseLost.AddListener(HandlePoseLost);
    }

    /// <summary>
    /// Clean up event listeners when component is disabled
    /// </summary>
    private void OnDisable()
    {
        if (handPoseDetector != null)
        {
            handPoseDetector.OnPoseDetected.RemoveListener(HandlePoseDetected);
            handPoseDetector.OnPoseLost.RemoveListener(HandlePoseLost);
        }
    }

    /// <summary>
    /// Initialize the component and start the learning sequence
    /// </summary>
    private void Start()
    {
        // Validate setup
        if (numberPoses == null || numberPoses.Length != 10)
        {
            Debug.LogError("Please assign exactly 10 number poses (0-9)!");
            return;
        }

        // Configure HandPoseDetector
        handPoseDetector.SetPosesToDetect(new List<HandPoseScriptableObject>(numberPoses));
        
        // Initialize UI
        successIcon.SetActive(false);
        completionPanel.SetActive(false);
        
        StartLearningSequence();
    }

    /// <summary>
    /// Begin the learning sequence from the first number
    /// </summary>
    private void StartLearningSequence()
    {
        currentNumberIndex = 0;
        ShowCurrentNumber();
    }

    /// <summary>
    /// Display the current number to learn
    /// </summary>
    private void ShowCurrentNumber()
    {
        if (currentNumberIndex >= numberPoses.Length)
        {
            ShowCompletion();
            return;
        }

        // Reset the hold timer when showing a new number
        ResetHoldTimer();

        // Update instruction text
        instructionText.text = $"Show and hold the sign for number {currentNumberIndex} for {requiredHoldTime} seconds";

        // Update optional image if assigned
        if (handSignImage != null && numberHandSignSprites != null && 
            currentNumberIndex < numberHandSignSprites.Length)
        {
            handSignImage.sprite = numberHandSignSprites[currentNumberIndex];
            handSignImage.gameObject.SetActive(true);
        }

        isWaitingForPose = true;
    }

    /// <summary>
    /// Handle when a pose is detected by the HandPoseDetector
    /// </summary>
    private void HandlePoseDetected()
    {
        if (!isWaitingForPose) return;

        // Get the currently detected pose
        HandPoseScriptableObject detectedPose = handPoseDetector.GetCurrentlyDetectedPose();
        if (detectedPose == null)
        {
            ResetHoldTimer();
            return;
        }

        // Clean up the pose name (remove "(Clone)" if present)
        string poseName = detectedPose.name.Replace("(Clone)", "").Trim();

        // Compare with expected pose
        if (detectedPose == numberPoses[currentNumberIndex])
        {
            isHoldingCorrectPose = true;
            // Note: We don't immediately handle success - we wait for the hold time
        }
        else
        {
            ResetHoldTimer();
            Debug.Log($"Wrong pose detected: {poseName}. Expected: {numberPoses[currentNumberIndex].name}");
        }
    }

    /// <summary>
    /// Reset the hold timer when pose is lost or changed
    /// </summary>
    private void ResetHoldTimer()
    {
        currentHoldTime = 0f;
        isHoldingCorrectPose = false;
        if (holdProgressBar != null)
        {
            holdProgressBar.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Handle successful detection after the pose has been held long enough
    /// </summary>
    private void HandleCorrectPose()
    {
        isWaitingForPose = false;

        // Cancel any existing success coroutine
        if (successCoroutine != null)
        {
            StopCoroutine(successCoroutine);
        }
        successCoroutine = StartCoroutine(ShowSuccessAndAdvance());
    }

    /// <summary>
    /// Display success animation and move to the next number
    /// </summary>
    private IEnumerator ShowSuccessAndAdvance()
    {
        // Show success icon
        successIcon.SetActive(true);
        
        yield return new WaitForSeconds(successIconDisplayTime);
        
        successIcon.SetActive(false);

        // Move to next number
        currentNumberIndex++;
        ShowCurrentNumber();
    }

    /// <summary>
    /// Show completion UI when all numbers have been learned
    /// </summary>
    private void ShowCompletion()
    {
        instructionText.text = "You're ready!";
        completionPanel.SetActive(true);
    }

    /// <summary>
    /// Restart the learning sequence from the beginning
    /// </summary>
    public void RestartSequence()
    {
        completionPanel.SetActive(false);
        StartLearningSequence();
    }

    /// <summary>
    /// Helper method to validate that pose names match their expected number names
    /// </summary>
    private void ValidatePoseNames()
    {
        string[] expectedNames = new string[] { 
            "zero", "one", "two", "three", "four", 
            "five", "six", "seven", "eight", "nine" 
        };

        for (int i = 0; i < numberPoses.Length; i++)
        {
            if (numberPoses[i] == null)
            {
                Debug.LogError($"Missing pose for number {i}!");
                continue;
            }

            string poseName = numberPoses[i].name.ToLower().Replace("(clone)", "").Trim();
            if (!poseName.Contains(expectedNames[i]))
            {
                Debug.LogWarning($"Pose {i} name '{poseName}' doesn't match expected '{expectedNames[i]}'");
            }
        }
    }

    /// <summary>
    /// Handle when a pose is lost by the HandPoseDetector
    /// </summary>
    private void HandlePoseLost()
    {
        ResetHoldTimer();
    }

    /// <summary>
    /// Update hold timer and check if pose has been held long enough
    /// </summary>
    private void Update()
    {
        // Only check hold time if we're waiting for a pose and currently holding the correct one
        if (isWaitingForPose && isHoldingCorrectPose)
        {
            currentHoldTime += Time.deltaTime;
            
            // Update progress bar if assigned
            if (holdProgressBar != null)
            {
                holdProgressBar.fillAmount = currentHoldTime / requiredHoldTime;
            }

            // Check if we've held long enough
            if (currentHoldTime >= requiredHoldTime)
            {
                HandleCorrectPose();
            }
        }
    }
} 