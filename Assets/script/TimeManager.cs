using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance {get; private set;}
    private int time; 
    private int day = 1;
    private int progress = 0;

    [SerializeField] private GameObject sun, moon;

    [SerializeField] GameState gameState;

    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Time Manager in scene.");
        }
        instance = this;
    }

    private void OnEnable() 
    {
        GameEventsManager.instance.timeEvents.onChangeTime += ChangeTime;
        GameEventsManager.instance.timeEvents.onChangeProgress += ChangeProgress;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.timeEvents.onChangeTime -= ChangeTime;
        GameEventsManager.instance.timeEvents.onChangeProgress -= ChangeProgress;
    }

    private void ChangeTime(int change)
    {
        time+=change;
        UpdateText();
        UpdateEvents();
        if (time >= 18) {
            time = 18;
        }
        else if (time <= 0) {
            time = 0;
        }
    }

    private void ChangeProgress(int progression)
    {
        progress+=progression;
        UpdateEvents();
    }

    public void SleepingEvent(int value)
    {
        time = 0;
        day += 1;
        sun.transform.eulerAngles = new Vector3(0, 15, 0);
        moon.transform.eulerAngles = new Vector3(0, -45, 0);
        UpdateEvents();
        ChangeTime(0);
    }

    void Start()
    {
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
        sun.transform.eulerAngles = new Vector3(0, 15, 0);
        moon.transform.eulerAngles = new Vector3(0, -45, 0);
        ChangeTime(0);
    }

    void Update()
    {
        if (sun.transform.eulerAngles.y < (time*15))
        {
            sun.transform.Rotate(Vector3.up * (2+(time*15)/sun.transform.eulerAngles.y) * Time.deltaTime);
            moon.transform.Rotate(Vector3.up * (2+(time*15)/moon.transform.eulerAngles.y) * Time.deltaTime);
        }
    }

    private void UpdateEvents() 
    {
        if (time >= 11 && day == 1 && progress == 1)
        {
            //Replace intro NPC with new NPC inside the house
            //GameObject.Find("Day1Tengu1").SetActive(false);
            GameObject.Find("Day1Tengu2").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (time >= 18 && day == 1)
        {
            //Instantiate Shuten
        }
    }

    private void UpdateText() 
    { 
        if (time <= 5) timerText.text = (6 + time).ToString() + ":00AM";
        else if (time == 6) timerText.text = (6 + time).ToString() + ":00PM";
        else if (time >= 18) timerText.text = ("12:00AM");
        else timerText.text = (time - 6).ToString() + ":00PM";
    }

    public int ReturnTime()
    {
        return time;
    }

    public int ReturnProgress()
    {
        return progress;
    }
}
