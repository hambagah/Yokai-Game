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
    //Player wakes up at 6:00AM 
    //Player should be asleep by 12:00PM

    [SerializeField] GameState gameState;

    void Start()
    {
        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
        sun.transform.eulerAngles = new Vector3(0, sun.transform.eulerAngles.y, 0);
        moon.transform.eulerAngles = new Vector3(0, moon.transform.eulerAngles.y, 0);
    }

    void Update()
    {
        if (time <= 5) timerText.text = (6 + time).ToString() + ":00AM";
        else if (time == 6) timerText.text = (6 + time).ToString() + ":00PM";
        else if (time >= 18) timerText.text = ("12:00AM");
        else timerText.text = (time - 6).ToString() + ":00PM";
        //
        //sun and moon are rotating based on time; 
        //
        if (Input.GetKeyDown(KeyCode.R)) UpdateTime(1);
        else if (Input.GetKeyDown(KeyCode.T)) UpdateTime(-1);
        
    }

    public void UpdateTime(int update) { 
        time += update;
        if (time >= 18) {
            time = 18;
            //If it's 18 it should be 12:00PM force the state of the game to enter limbo, player must sleep. 
        }
        sun.transform.eulerAngles = new Vector3(time * 15, sun.transform.eulerAngles.y, 0);
        moon.transform.eulerAngles = new Vector3(time * 8, moon.transform.eulerAngles.y, 0);
        //should the angle gradually adjust to the new time?
    }

    public void ResetDay() 
    {
        time = 0;
        gameState.day += 1;
    }
}
