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
    public int time; 
    public int day = 1;
    public int progress = 0;
    public float stepMid = 0;
    public float stepEnd = 0;
    private Color colorStart = new Color32(128, 128, 128, 128);
    private Color colorMid = new Color32(212, 125, 85, 128);
    private Color colorEnd = new Color32(27, 15, 58, 128);
    [SerializeField] private Material skyboxTexture;

    [SerializeField] private GameObject sun, moon;

    [SerializeField] GameState gameState;

    [SerializeField] private TextMeshProUGUI timerText;

    private bool spawnShuten = false;
    Player player;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Time Manager in scene.");
        }
        instance = this;
        RenderSettings.skybox = new Material(skyboxTexture);
        colorStart = new Color32(128, 128, 128, 128);
        colorMid = new Color32(212, 125, 85, 128);
        colorEnd = new Color32(27, 15, 58, 128);
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
        Debug.Log(sceneId);
        if (sceneId.Equals("Mixing"))
        {
            progress = 3;
            SaveTime(time, day, progress);
            player = GameObject.Find("Player").GetComponent<Player>();
            player.SavePlayer(player.transform.position);
            SceneManager.LoadScene("The Mixing Game Demo");
        }

        if (sceneId.Equals("Rhythm")) 
        {
            progress = 4;
            SaveTime(time, day, progress);
            player = GameObject.Find("Player").GetComponent<Player>();
            player.SavePlayer(player.transform.position);
            SceneManager.LoadScene("Rhythm");
        }

        if (sceneId.Equals("Spawn Shuten"))
        {
            spawnShuten = true;
            UpdateEvents();
        }

        if (sceneId.Equals("Third"))
        {
            progress = 5;
            SaveTime(time, day, progress);
            player = GameObject.Find("Player").GetComponent<Player>();
            player.SavePlayer(player.transform.position);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Couning Scene");
        }
    }

    void Start()
    {
        sun = GameObject.Find("Sun");
        moon = GameObject.Find("Moon");
        if (!loadTimeState)
        {
            sun.transform.eulerAngles = new Vector3(0, 15, 0);
            moon.transform.eulerAngles = new Vector3(0, -45, 0);
            ChangeTime(0);
        }
    }

    void Update()
    {
        if (loadTimeState)
        {
            LoadTime();
            sun.transform.eulerAngles = new Vector3(0, 15 + time*15, 0);
            moon.transform.eulerAngles = new Vector3(0, -45 + time * 10, 0);
            ChangeTime(0);
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            ChangeTime(1);    
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            SceneManager.LoadScene("PlayingScene Initial");
        }

        if (time > 9) {
             RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorMid, colorEnd, time/9));
        }
        else if (time > 1) {
             RenderSettings.skybox.SetColor("_Tint", Color.Lerp(colorStart, colorMid, time/9));
        }

        if (sun.transform.eulerAngles.y < (time*15))
        {
           sun.transform.Rotate(Vector3.up * (2+(time*15)/sun.transform.eulerAngles.y) * Time.deltaTime);
           moon.transform.Rotate(Vector3.up * (2+(time*10)/moon.transform.eulerAngles.y) * Time.deltaTime);
        }
    }

    private void UpdateEvents() 
    {
        if (time == 0 && day == 1 && progress < 2) //DAY 1
        {
            //Load 5 Boxes
            //Load 3 Areas
            //Load Tengu, Fox, and Shuten
            GameObject.Find("Day1Boxes").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Day1Checks").transform.GetChild(0).gameObject.SetActive(true);
        }

        // if (time >= 0 && day >= 0 && progress >= 0)
        // {
        //     GameObject.Find("Day2Tamamo").transform.GetChild(0).gameObject.SetActive(true);
        //     GameObject.Find("Day2Tanuki").transform.GetChild(0).gameObject.SetActive(true);
        //     GameObject.Find("Day1Shuten").transform.GetChild(0).gameObject.SetActive(true);

        // }

        if (time >= 11 && day == 1 && progress == 1) //After the player collects 5 boxes and interacts with Tengu. 
        {
            //Destroy(GameObject.Find("Day1Boxes"));
            GameObject.Find("Day1Tamamo").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (time >= 12) //After the player interacts with Fox
        {
            GameObject.Find("Day1Tengu1").transform.GetChild(0).gameObject.SetActive(false);
        }

        if (time >= 18 && day == 1 && (spawnShuten || progress == 2)) //At night time.
        {
            //Instantiate Shuten
            GameObject.Find("Day1Shuten").transform.GetChild(0).gameObject.SetActive(true);
            progress = 2;
        }

        if (time >= 18 && day == 1 && progress == 3) //At night time.
        {
            //Instantiate Shuten
            GameObject.Find("Day1Shuten2").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Day1Shuten").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Bed").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Bed2").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (day == 2 && progress < 4)
        {
            GameObject.Find("Day2Tamamo").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (day == 2 && progress > 3)
        {
            GameObject.Find("Day2Tamamo").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Day2Tamamo2").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("Day2Tanuki").transform.GetChild(0).gameObject.SetActive(true);
        }

        if (day == 2 && progress > 4)
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

    public int ReturnDay()
    {
        return day;
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
