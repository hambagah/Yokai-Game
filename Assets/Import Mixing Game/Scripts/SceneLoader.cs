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
     * Generic method to return from any minigame to the main game
     * with proper progress saved
     * 
     * @param day The day to set (usually incremented after minigame)
     * @param progress The progress value to set after completing minigame
     * @param time Optional time of day (0 = morning, default)
     */
    public void ReturnToMainGame(int day, int progress, int time = 0)
    {
        Debug.Log($"ReturnToMainGame: Setting day={day}, progress={progress}, time={time}");
        
        // Save the game state to indicate completion of the minigame
        PlayerPrefs.SetInt("Time", time);
        PlayerPrefs.SetInt("Day", day);
        PlayerPrefs.SetInt("Progress", progress);
        
        // Force the TimeManager to load state from PlayerPrefs 
        // by setting this special flag to true
        PlayerPrefs.SetInt("ForceLoadTimeState", 1);
        
        // Make sure changes are saved immediately
        PlayerPrefs.Save();
        
        // Load the main game scene - use PlayingSceneStart not PlayingScene Initial
        SceneManager.LoadScene("PlayingScene");
    }
    
    /**
     * Return from the Mixing Game specifically (day 2, progress 4)
     * This is kept for backward compatibility
     */
    public void ReturnFromMixingGame()
    {
        Debug.Log("ReturnFromMixingGame: Setting day=2, progress=0");
        ReturnToMainGame(2, 0, 0);
    }
    
    /**
     * Return from the Rhythm Game specifically (day 2, progress 5)
     */
    public void ReturnFromRhythmGame()
    {
        Debug.Log("ReturnFromRhythmGame: Setting day=2, progress=5");
        ReturnToMainGame(2, 5, 0);
    }
    
    /**
     * Return from the Math/Counting Game (final state)
     */
    public void ReturnFromMathGame()
    {
        Debug.Log("ReturnFromMathGame: Setting day=3, progress=6");
        ReturnToMainGame(3, 6, 0);
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
