using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0; // 当前得分
    public int targetScore = 100; // 通过关卡的最低分数
    public TextMeshProUGUI scoreText; // UI 显示分数
    public GameObject gameOverPanel; // 失败面板
    public GameObject successPanel; // 成功面板
    public AudioSource backgroundMusic; // 背景音乐

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (successPanel) successPanel.SetActive(false);
    }

    public void AddScore(Tail.TailType tailType)
    {
        int points = 0;

        switch (tailType)
        {
            case Tail.TailType.Tap:
                points = 10;
                break;
            case Tail.TailType.Hold:
                points = 20;
                break;
            case Tail.TailType.Slide:
                points = 30;
                break;
        }

        score += points;
        CheckGameResult();

        UpdateScoreUI();
        Debug.Log($"Scored {points} points! Total score: {score}");
    }

    void UpdateScoreUI()
    {
        if (scoreText)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void CheckGameResult()
    {
        if (gameEnded) return;
        if (backgroundMusic.isPlaying)
            return;
        gameEnded = true;

        if (score >= targetScore)
        {
            Debug.Log("挑战成功!");
            successPanel.SetActive(true);
        }
        else
        {
            Debug.Log("挑战失败!");
            gameOverPanel.SetActive(true);
        }
    }
}
