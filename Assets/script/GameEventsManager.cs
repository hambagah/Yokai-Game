using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance {get; private set;}

    public InputEvents inputEvents;
    public PlayerEvents playerEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;
    public GoldEvents goldEvents;
    public DialogueEvents dialogueEvents;
    //public TimeEvents timeEvents;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        instance = this;

        inputEvents = new InputEvents();
        playerEvents = new PlayerEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        goldEvents = new GoldEvents();
        dialogueEvents = new DialogueEvents();
        //timeEvents = new TimeEvents();
    }
}
