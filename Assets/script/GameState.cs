using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int progress;
    public Timer time; 
    public Vector3 playerPos; 
    void Start()
    {
        progress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (progress == 0)
        {
            
        }
    }

    public void Progress(int update)
    {
        progress += update;
    }

    public void Time(int tUpdate)
    {
        time.UpdateTime(tUpdate);
    }
}
