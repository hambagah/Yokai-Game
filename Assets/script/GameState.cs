using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int progress;
    public int day;
    public Timer time; 
    public Vector3 playerPos; 
    [SerializeField] GameObject NPC;
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
        if (day == 2) {
            NPC.SetActive(true);
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
