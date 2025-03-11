using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
public class VisitSpotsQuestStep : QuestStep
{
    private int boxesCleared = 0;

    private int boxesToClear = 3;

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.objectCleaned += ObjectCleared;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.objectCleaned -= ObjectCleared;
    }

    private void ObjectCleared()
    {
        if (boxesCleared < boxesToClear)
        {
            boxesCleared++;
            UpdateState();
        }

        if (boxesCleared >= boxesToClear)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = boxesCleared.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.boxesCleared = System.Int32.Parse(state);
        UpdateState();
    }

    /*private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player") && TimeManager.instance.ReturnProgress() == 1)
        {
            FinishQuestStep();
        }
    }

    protected override void SetQuestStepState(string state)
    {

    }*/
}
