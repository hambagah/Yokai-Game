using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager st;
    [Serializable]
    public struct EndInfoItem
    {
        public TextMeshProUGUI countText;
        public TextMeshProUGUI scoreText;
    }
    [SerializeField] EndInfoItem[] endInfoItems;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject successPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Image bgImage;
    [SerializeField] Sprite successBGSprite, gameOverBGSprite;
    [SerializeField] TextMeshProUGUI totalScoreText;
    [SerializeField] Button restartButton;
    [SerializeField] Button okButton;
    private void Awake()
    {
        st = this;
        restartButton.onClick.AddListener(RestartGame);
        okButton.onClick.AddListener(OK);
    }
    public void ShowEndPanel(GameInfo gameInfo)
    {
        for(int i=0;i< gameInfo.gradeCount.Length;i ++)
        {
            endInfoItems[i].countText.text = gameInfo.gradeCount[i].ToString();
            endInfoItems[i].scoreText.text = gameInfo.gradeScore[i].ToString();
        }
        endPanel.SetActive(true);

        totalScoreText.text = gameInfo.totalScore.ToString();
        if(gameInfo.success)
            successPanel.SetActive(true);
        else
            gameOverPanel.SetActive(true);

        bgImage.sprite = gameInfo.success ? successBGSprite : gameOverBGSprite;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OK()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
