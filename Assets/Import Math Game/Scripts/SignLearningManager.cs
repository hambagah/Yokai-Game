using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap;
using System;

public class SignLearningManager : MonoBehaviour
{
    [Header("Pose Detection")]
    [SerializeField] private HandPoseDetector handPoseDetector;
    [SerializeField] private HandPoseScriptableObject[] numberPoses; // Array of 10 poses (0-9)
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image handSignImage;
    [SerializeField] private GameObject successIcon;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private float successIconDisplayTime = 1.0f;
    
    [Header("Optional")]
    [SerializeField] private Sprite[] numberHandSignSprites;

    [Header("Gesture Timing")]
    [SerializeField] private float requiredHoldTime = 2.0f; // Time in seconds to hold the gesture
    [SerializeField] private Image holdProgressBar; // Optional: Visual feedback of hold progress
    
    private int currentNumberIndex = 0;
    private bool isWaitingForPose = false;
    private Coroutine successCoroutine;
    private float currentHoldTime = 0f;
    private bool isHoldingCorrectPose = false;

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

    private void OnDisable()
    {
        if (handPoseDetector != null)
        {
            handPoseDetector.OnPoseDetected.RemoveListener(HandlePoseDetected);
            handPoseDetector.OnPoseLost.RemoveListener(HandlePoseLost);
        }
    }

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

    private void StartLearningSequence()
    {
        currentNumberIndex = 0;
        ShowCurrentNumber();
    }

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

    private void ResetHoldTimer()
    {
        currentHoldTime = 0f;
        isHoldingCorrectPose = false;
        if (holdProgressBar != null)
        {
            holdProgressBar.fillAmount = 0f;
        }
    }

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

    private void ShowCompletion()
    {
        instructionText.text = "You're ready!";
        completionPanel.SetActive(true);
    }

    public void RestartSequence()
    {
        completionPanel.SetActive(false);
        StartLearningSequence();
    }

    // Optional: Helper method to validate pose names match their indices
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

    // Add this method to handle when the pose is lost
    private void HandlePoseLost()
    {
        ResetHoldTimer();
    }

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