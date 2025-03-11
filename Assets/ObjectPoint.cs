using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectPoint : MonoBehaviour //Can potentially migrate to Interactable
{
    [Header("Dialogue")]
    [SerializeField] private string dialogueKnotName;
    
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [SerializeField] private GameObject questIcon;

    private bool playerIsNear = false;
    public int objectType;

    private void Awake()
    {
        questIcon.SetActive(false);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
        {
            return;
        }

        if (!dialogueKnotName.Equals(""))
        {
            //if (objectType) { //Cleaning and destroying boxes
                //Debug.Log("Yes");
                GameEventsManager.instance.dialogueEvents.ObjectDialogue(dialogueKnotName, this.gameObject);
                //GameEventsManager.instance.dialogueEvents.ObjectDialogue(dialogueKnotName, this.gameObject);
                //Cleaned();
                //Day1CleanBoxes(Clone);
            //}
        }
    }
    
    public void Cleaned()
    {
        GameEventsManager.instance.miscEvents.ObjectCleaned();
        GameEventsManager.instance.timeEvents.ChangeTime(2);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && GameObject.Find("CleanBoxesQuest(Clone)") && objectType == 1)//Boxes
        {
            questIcon.SetActive(true);
            playerIsNear = true;
        }
        if (collider.CompareTag("Player") && TimeManager.instance.ReturnTime() >= 10 && objectType == 2)//Bed
        {
            questIcon.SetActive(true);
            playerIsNear = true;
        }
        if (collider.CompareTag("Player") && GameObject.Find("VisitSpotsQuest(Clone)") && objectType == 1)//Boxes
        {
            questIcon.SetActive(true);
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            questIcon.SetActive(false);
            playerIsNear = false;
        }
    }
}
