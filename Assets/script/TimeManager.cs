using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance {get; private set;}
    [Header("Config")]
    [SerializeField] private bool loadTimeState = true;
    private int time; 
    private int day = 1;
    private int progress = 0;
    public float stepMid = 0;
    public float stepEnd = 0;
    private Color colorStart = new Color(128, 128, 128, 128);


    [SerializeField] private GameObject sun, moon;

    [SerializeField] GameState gameState;

    [SerializeField] private TextMeshProUGUI timerText;

    private bool spawnShuten = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Time Manager in scene.");
        }
        instance = this;
        // colorStart = new Color(169, 169, 169);
        // colorMid = new Color(212, 125, 85);
        // colorEnd = new Color(37, 54, 99);
        // RenderSettings.skybox.SetColor("_Tint", colorStart);
        RenderSettings.skybox.SetColor("_Tint", colorStart);
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
        Debug.Log(value);
        if (value == 1)
        {
            spawnShuten = true;
            UpdateEvents();
        }
        else {
            time = 0;
            day += 1;
            sun.transform.eulerAngles = new Vector3(0, 15, 0);
            moon.transform.eulerAngles = new Vector3(0, -45, 0);
            UpdateEvents();
            ChangeTime(0);
        }
    }

    public void SceneEvent(string sceneId)
    {
        if (sceneId.Equals("The Mixing Game Demo"))
            SceneManager.LoadScene("The Mixing Game Demo");

        if (sceneId.Equals("Rhythm"))
            SceneManager.LoadScene("Rhythm");

        if (sceneId.Equals("Spawn Shuten"))
        {
            spawnShuten = true;
            UpdateEvents();
        }
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
        if (loadTimeState)
        {
            LoadTime();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            ChangeTime(1);    
        }

        // if (time > 9) {
        //     RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorMid, colorEnd, time));
        // }
        // else {
        //     RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorStart, colorMid, time));
        // }

        if (sun.transform.eulerAngles.y < (time*15))
        {
            sun.transform.Rotate(Vector3.up * (2+(time*15)/sun.transform.eulerAngles.y) * Time.deltaTime);
            moon.transform.Rotate(Vector3.up * (2+(time*15)/moon.transform.eulerAngles.y) * Time.deltaTime);
        }
    }

    private void UpdateEvents() 
    {
        if (time == 0 && day == 1 && progress == 1) //DAY 1
        {
            //Load 5 Boxes
            //Load 3 Areas
            //Load Tengu, Fox, and Shuten
        }

        if (time >= 11 && day == 1 && progress == 1) //After the player collects 5 boxes and interacts with Tengu. 
        {
            //Destroy(GameObject.Find("Day1Boxes"));
            GameObject.Find("Day1Tengu2").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (time >= 12) //After the player interacts with Fox
        {
            GameObject.Find("Day1Tengu1").transform.GetChild(0).gameObject.SetActive(false);
        }

        if (time >= 18 && day == 1 && spawnShuten) //At night time.
        {
            //Instantiate Shuten
            GameObject.Find("Day1Shuten").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (time == 0 && day == 2)
        {

        }

        if (time >= 0 && day ==2 && progress == 2)
        {

        }

        if (time >= 0 && day == 3 && progress >= 3)
        {

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

    private void OnApplicationQuit()
    {
        SaveTime(time, day, progress);
    }

    private void SaveTime(int time, int day, int progress)
    {
        try 
        {
            PlayerPrefs.SetInt("Time", time);
            PlayerPrefs.SetInt("Day", day);
            PlayerPrefs.SetInt("Progress", progress);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save time progress");
        }
    }

    private void LoadTime()
    {
        try 
        {
            int savedTime = PlayerPrefs.GetInt ("Time");
            int savedDay = PlayerPrefs.GetInt ("Day");
            int savedProgress = PlayerPrefs.GetInt ("Progress");
            time = savedTime;
            day = savedDay;
            progress = savedProgress;
            loadTimeState = false;
            Debug.Log("Loaded Time: " + savedTime + " Loaded Day: " + savedDay + " Loaded Progress: " + savedProgress);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Time failed");
        }
    }
}
