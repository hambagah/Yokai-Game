using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class CountingGameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private ObjectSpawnerManager objectSpawner;
    [SerializeField] private HandPoseDetector handPoseDetector;
    [SerializeField] private HandPoseScriptableObject[] numberPoses; // 0-9 poses
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button nextRoundButton;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    
    [Header("Game Settings")]
    [SerializeField] private int totalRounds = 5;
    [SerializeField] private float answerTimeLimit = 10f;
    [SerializeField] private float requiredHoldTime = 2.0f;
    [SerializeField] private Image holdProgressBar;
    
    private int currentRound = 0;
    private int score = 0;
    private bool isWaitingForAnswer = false;
    private float currentHoldTime = 0f;
    private bool isHoldingCorrectPose = false;
    private int expectedAnswer = 0;
    private Coroutine timerCoroutine;
    private GameState currentState = GameState.Idle;

    private enum GameState
    {
        Idle,
        Spawning,
        WaitingForPlayerAnswer,
        ShowingResults
    }

    private void OnEnable()
    {
        objectSpawner.OnSpawnFinished += HandleSpawnComplete;
        handPoseDetector.OnPoseDetected.AddListener(HandlePoseDetected);
        handPoseDetector.OnPoseLost.AddListener(HandlePoseLost);
        
        if (nextRoundButton != null)
            nextRoundButton.onClick.AddListener(StartNextRound);
    }

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

    private void Update()
    {
        if (currentState == GameState.WaitingForPlayerAnswer && isHoldingCorrectPose)
        {
            currentHoldTime += Time.deltaTime;
            
            if (holdProgressBar != null)
            {
                holdProgressBar.fillAmount = currentHoldTime / requiredHoldTime;
            }

            if (currentHoldTime >= requiredHoldTime)
            {
                CheckAnswer();
            }
        }
    }

    public void StartGame()
    {
        currentRound = 0;
        score = 0;
        currentState = GameState.Idle;
        StartCoroutine(StartRoundWithCountdown());
    }

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

    private void StartRound()
    {
        currentRound++;
        if (currentRound > totalRounds)
        {
            EndGame();
            return;
        }
        
        roundText.text = $"Round {currentRound}/{totalRounds}";
        instructionText.text = $"Watch and count the {objectSpawner.targetObjectName}s!";
        
        currentState = GameState.Spawning;
        objectSpawner.BeginSpawning();
        
        ResetHoldTimer();
        isWaitingForAnswer = false;
    }

    private void HandleSpawnComplete()
    {
        expectedAnswer = objectSpawner.GetTargetObjectCount();
        currentState = GameState.WaitingForPlayerAnswer;
        
        // Wait for player's answer
        instructionText.text = $"How many {objectSpawner.targetObjectName}s did you count? Show with your hand sign.";
        isWaitingForAnswer = true;
        
        // Show hold progress bar
        if (holdProgressBar != null)
            holdProgressBar.gameObject.SetActive(true);
        
        // Start timer for answer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
            
        timerCoroutine = StartCoroutine(AnswerTimeLimit());
    }

    private IEnumerator AnswerTimeLimit()
    {
        yield return new WaitForSeconds(answerTimeLimit);
        
        if (currentState == GameState.WaitingForPlayerAnswer)
        {
            isWaitingForAnswer = false;
            ShowResult(false, "Time's up!");
        }
    }

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

        // Check if it's the expected answer
        if (detectedNumber == expectedAnswer)
        {
            isHoldingCorrectPose = true;
            instructionText.text = $"Hold the sign for {expectedAnswer}...";
        }
        else
        {
            ResetHoldTimer();
            instructionText.text = $"That's {detectedNumber}, not the correct count. Try again!";
        }
    }

    private void HandlePoseLost()
    {
        ResetHoldTimer();
        
        if (currentState == GameState.WaitingForPlayerAnswer)
        {
            instructionText.text = $"How many {objectSpawner.targetObjectName}s did you count? Show with your hand sign.";
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

    private void CheckAnswer()
    {
        if (currentState != GameState.WaitingForPlayerAnswer) return;
        
        isWaitingForAnswer = false;
        
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        // Correct answer
        score++;
        ShowResult(true, "Correct!");
    }

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

    public void StartNextRound()
    {
        resultPanel.SetActive(false);
        objectSpawner.ClearObjects();
        StartCoroutine(StartRoundWithCountdown());
    }

    private void EndGame()
    {
        currentState = GameState.ShowingResults;
        resultPanel.SetActive(true);
        resultText.text = $"Game Over!\nYour final score: {score}/{totalRounds}";
        
        // Option to restart game
        nextRoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";
    }
} 