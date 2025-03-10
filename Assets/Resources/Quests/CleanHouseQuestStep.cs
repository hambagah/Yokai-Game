using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanHouseQuestStep : QuestStep
{
    private int boxesCleared = 0;

    private int boxesToClear = 5;

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
}
