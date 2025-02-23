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
        }

        if (boxesCleared >= boxesToClear)
        {
            FinishQuestStep();
        }
    }
}
