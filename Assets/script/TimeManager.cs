using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance {get; private set;}
    /*[Header("Config")]
    [SerializeField] private bool loadQuestState = true;*/
    private int time; 
    private int day = 1;
    private int progress = 0;

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

        if (sceneId.Equals("Spawn Shuten"))
            spawnShuten = true;
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

        if (time >= 18 && day == 1 && spawnShuten)
        {
            //Instantiate Shuten
            GameObject.Find("Day1Shuten").transform.GetChild(0).gameObject.SetActive(true);
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
/*
    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveTime(quest);
        }
    }

    private void SaveTime(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();
            // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
            string serializedData = JsonUtility.ToJson(questData);
            // saving to PlayerPrefs is just a quick example for this tutorial video,
            // you probably don't want to save this info there long-term.
            // instead, use an actual Save & Load system and write to a file, the cloud, etc..
            PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadTime(QuestInfoSO questInfo)
    {
        Quest quest = null;
        Debug.Log($"Load Quest call: {questInfo.id}");
        try 
        {
            // load quest from saved data
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else 
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }*/
}
