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
        progress = 0;
        day = 1;
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
        TimeManager.GetInstance().UpdateTime(tUpdate);
    }
}
