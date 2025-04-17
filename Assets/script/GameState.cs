using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int progress;
    public int day;
    public Vector3 playerPos; 
    
    void Start()
    {
        // Only initialize to default values if PlayerPrefs doesn't have stored values
        // or if we're not coming back from a minigame
        if (!PlayerPrefs.HasKey("Day") || !PlayerPrefs.HasKey("Progress") || 
            !PlayerPrefs.HasKey("ForceLoadTimeState"))
        {
            progress = 0;
            day = 1;
            Debug.Log("GameState initialized with default values: day=1, progress=0");
        }
        else
        {
            // Load from PlayerPrefs directly if TimeManager hasn't set these yet
            if (PlayerPrefs.HasKey("Day"))
                day = PlayerPrefs.GetInt("Day");
                
            if (PlayerPrefs.HasKey("Progress"))
                progress = PlayerPrefs.GetInt("Progress");
                
            Debug.Log("GameState loaded from PlayerPrefs: day=" + day + ", progress=" + progress);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (progress == 0)
        {
            
        }
    }

    public void Spawn() {
    }

    public void Progress(int update)
    {
        progress += update;
    }

    public void Time(int tUpdate)
    {
        //TimeManager.GetInstance().UpdateTime(tUpdate);
    }
}
