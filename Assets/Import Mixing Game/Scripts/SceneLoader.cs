using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void StartGameplay(string stageToPlay)
    {
        SceneManager.LoadScene(stageToPlay);
    }

    // Reload the current scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();

        // exit game in unity editor
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
