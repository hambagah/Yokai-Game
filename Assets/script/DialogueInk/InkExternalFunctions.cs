using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
        story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
        story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
    }

    private void StartQuest(string questId)
    {
        GameEventsManager.instance.questEvents.StartQuest(questId);
    }

    private void AdvanceQuest(string questId)
    {
        GameEventsManager.instance.questEvents.AdvanceQuest(questId);
    }

    private void FinishQuest(string questId)
    {
        GameEventsManager.instance.questEvents.FinishQuest(questId);
    }

    /*
    public void Bind(Story story, Animator emoteAnimator)
    {
        story.BindExternalFunction("playEmote", 
            (string emoteName) => PlayEmote(emoteName, emoteAnimator));


        story.BindExternalFunction("startQuest", 
            (string questId) => StartQuest(questId));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("playEmote");
    }

    public void PlayEmote(string emoteName, Animator emoteAnimator)
    {
        if (emoteAnimator != null) 
        {
            emoteAnimator.Play(emoteName);
        }
        else 
        {
            Debug.LogWarning("Tried to play emote, but emote animator was "
                + "not initialized when entering dialogue mode.");
        }
    }

    public void StartQuest(string questId)
    {
        Debug.Log(questId);
        if (questId == "sleeping") TimeManager.GetInstance().ResetDay(); 
    }*/
}
