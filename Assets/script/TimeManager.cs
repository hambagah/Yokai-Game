using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TimeManager : MonoBehaviour
{
    //Player wakes up at 6:00AM 
    //Player should be asleep by 12:00PM
    private int time; 
    private int day;
    [SerializeField] private GameObject sun, moon;

    [SerializeField] GameState gameState;
    [SerializeField] private TextMeshProUGUI timerText;

    private static TimeManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Time Manager in scene.");
        }
        instance = this;
    }

    public static TimeManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
        sun.transform.eulerAngles = new Vector3(0, sun.transform.eulerAngles.y, 0);
        moon.transform.eulerAngles = new Vector3(0, moon.transform.eulerAngles.y, 0);
        UpdateTime(0);
    }

    void Update()
    {
        //
        //sun and moon are rotating based on time; 
        //
        if (Input.GetKeyDown(KeyCode.Y)) UpdateTime(1);
        else if (Input.GetKeyDown(KeyCode.U)) UpdateTime(-1);
    }

    public void UpdateTime(int update) { 
        time += update;
        if (time >= 18) {
            time = 18;
            //If it's 18 it should be 12:00PM force the state of the game to enter limbo, player must sleep. 
        }
        else if (time <= 0) {
            time = 0;
        }
        sun.transform.eulerAngles = new Vector3(time * 15, sun.transform.eulerAngles.y, 0);
        moon.transform.eulerAngles = new Vector3(time * 8, moon.transform.eulerAngles.y, 0);

        if (time <= 5) timerText.text = (6 + time).ToString() + ":00AM";
        else if (time == 6) timerText.text = (6 + time).ToString() + ":00PM";
        else if (time >= 18) timerText.text = ("12:00AM");
        else timerText.text = (time - 6).ToString() + ":00PM";
    }

    public int GetTime()
    {
        return time;
    }

    public void ResetDay() 
    {
        time = 0;
        gameState.day += 1;
        gameState.Spawn();
        UpdateTime(0);
    }
}
