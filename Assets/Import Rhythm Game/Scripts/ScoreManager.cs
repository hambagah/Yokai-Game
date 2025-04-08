using System;
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
    [SerializeField] AudioSource sfxAu;
    [SerializeField] AudioClip hitClip;
    [SerializeField] GameObject hitFX;
    private bool gameEnded = false;

    public float delayTime = 5f;
    public float offsetTime = 0f;

    public float missTime => delayTime + offsetTime + level[level.Length-1];
    public float[] level;
    [SerializeField] int[] scores;

    [SerializeField] GameInfo gameInfo;

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

    public void OnHit(NoteInfo info)
    {
        sfxAu.PlayOneShot(hitClip);
       GameObject fx =  Instantiate(hitFX, info.pos, Quaternion.identity);
        Destroy(fx, 4f);
        int curLevel = GetScore(Mathf.Abs(delayTime - info.curTime + offsetTime));
        score += scores[curLevel];

        DamageNumManager.st.Create((NoteHitType)curLevel, info.pos);
        UpdateScoreUI();
        AddGrade(curLevel);
    }
    public void AddGrade(int level)
    {
        gameInfo.gradeCount[level]++;
        gameInfo.gradeScore[level]+= scores[level];
        gameInfo.totalScore += scores[level];
    }

    int GetScore(float value)
    {
        Debug.Log("value" + value);
        for(int i=0; i < level.Length -1;i++)
        {
            if(value >= level[i] && value < level[i+1])
            {
                return i;
            }
        }

        return level.Length;
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }

    public void EndGame()
    {
        gameInfo.success = score >= targetScore;
        UIManager.st.ShowEndPanel(gameInfo);
    }
}
[Serializable]
public struct NoteInfo
{
    public Vector3 pos;
    public float curTime;
}

public enum NoteHitType
{
    Perfect,
    Good,
    Bad,
    Miss,
}
[Serializable]
public struct GameInfo
{
    public bool success;
    public int[] gradeCount;
    public int[] gradeScore;
    public int totalScore;
}