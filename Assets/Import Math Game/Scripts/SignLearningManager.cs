using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SignLearningManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Image handSignImage; // Optional
    [SerializeField] private GameObject successIcon;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private float successIconDisplayTime = 1.0f;

    [Header("Pose Detection")]
    [SerializeField] private SignPoseDetector[] numberPoseDetectors; // Array of 10 detectors (0-9)
    
    [Header("Optional")]
    [SerializeField] private Sprite[] numberHandSignSprites; // Optional array of hand sign images

    private int currentNumberIndex = 0;
    private bool isWaitingForPose = false;
    private Coroutine detectionCoroutine;

    private void Start()
    {
        // Ensure all detectors are disabled at start
        foreach (var detector in numberPoseDetectors)
        {
            detector.SetActive(false);
        }

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
        if (currentNumberIndex >= numberPoseDetectors.Length)
        {
            ShowCompletion();
            return;
        }

        // Update instruction text
        instructionText.text = $"Show the sign for number {currentNumberIndex}";

        // Update optional image if assigned
        if (handSignImage != null && numberHandSignSprites != null && 
            currentNumberIndex < numberHandSignSprites.Length && 
            numberHandSignSprites[currentNumberIndex] != null)
        {
            handSignImage.sprite = numberHandSignSprites[currentNumberIndex];
            handSignImage.gameObject.SetActive(true);
        }

        // Activate current detector
        numberPoseDetectors[currentNumberIndex].SetActive(true);
        isWaitingForPose = true;

        // Stop existing coroutine if any
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
        }
        detectionCoroutine = StartCoroutine(CheckForPoseDetection());
    }

    private IEnumerator CheckForPoseDetection()
    {
        // Wait a brief moment to allow the pose detector to initialize
        yield return new WaitForSeconds(0.1f);
        
        while (isWaitingForPose)
        {
            if (numberPoseDetectors[currentNumberIndex].HasBeenDetected)
            {
                yield return HandleSuccessfulPose();
                break;
            }
            yield return null;
        }
    }

    private IEnumerator HandleSuccessfulPose()
    {
        isWaitingForPose = false;
        
        // Show success icon
        successIcon.SetActive(true);
        
        // Wait a moment with the success icon showing
        yield return new WaitForSeconds(successIconDisplayTime);
        
        // Hide success icon and deactivate current detector
        successIcon.SetActive(false);
        numberPoseDetectors[currentNumberIndex].SetActive(false);

        // Move to next number
        currentNumberIndex++;
        ShowCurrentNumber();
    }

    private void ShowCompletion()
    {
        instructionText.text = "You're ready!";
        completionPanel.SetActive(true);
    }

    // Optional: Method to be called by a UI button to restart the sequence
    public void RestartSequence()
    {
        completionPanel.SetActive(false);
        StartLearningSequence();
    }
} 