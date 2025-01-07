using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    /*public bool playerTurn = false;
    public float timer;
    private float endTurnTime;*/
    public int time; 
    private TMP_Text timerText;
    private GameObject sun, moon;
    //Player wakes up at 7:00AM 
    //Player should be asleep by 12:00PM

    void Start()
    {
        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
    }

    void Update()
    {
        timerText.text = (7 + time).ToString("#.");
        //
        //sun and moon are rotating based on time; 
        //
        if (Input.GetKeyDown(KeyCode.R)) UpdateTime(1);
        else if (Input.GetKeyDown(KeyCode.T)) UpdateTime(-1);
        
    }

    public void UpdateTime(int update) { 
        time += update;
        if (time >= 17) {
            //If it's 17 it should be 12:00PM force the state of the game to enter limbo, player must sleep. 
        }
    }
}
