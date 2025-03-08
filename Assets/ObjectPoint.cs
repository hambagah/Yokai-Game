using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectPoint : MonoBehaviour //Can potentially migrate to Interactable
{
    /*[Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;*/

    //private bool oops;
    private bool playerIsNear = false;
    //private string questId;

    //private QuestState currentQuestState;
    private QuestIcon questIcon;
    public int objectType;

    private void Awake()
    {
        //questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void OnEnable()
    {
        //GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        //GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!playerIsNear)
        {
            return;
        }

        if (objectType == 0) { //Cleaning and destroying boxes
            Cleaned();
        }
    }
    
    public void Cleaned()
    {
        GameEventsManager.instance.miscEvents.ObjectCleaned();
        Destroy(gameObject);
    }

    /*private void QuestStateChange(Quest quest) 
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("Quest with id: " + questId + " updated to state: " + currentQuestState);
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
    }*/

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
