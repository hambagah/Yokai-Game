/**
 * SceneLoader.cs
 * 
 * Summary: Manages scene transitions and game flow.
 * Provides methods to load specific scenes, reload the current scene,
 * and exit the game.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /**
     * Loads a specific scene by name
     * @param stageToPlay The name of the scene to load
     */
    public void StartGameplay(string stageToPlay)
    {
        SceneManager.LoadScene(stageToPlay);
    }

    /**
     * Reloads the current active scene
     * Useful for resetting the game state
     */
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /**
     * Exits the game application
     * Works in both built games and the Unity Editor
     */
    public void ExitGame()
    {
        // Exit in standalone build
        Application.Quit();

        // Exit in Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
