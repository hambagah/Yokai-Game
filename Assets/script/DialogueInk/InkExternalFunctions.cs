using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

/// <summary>
/// Connects Ink narrative functions to Unity gameplay systems.
/// Allows the narrative to trigger game events like quests, scene changes, etc.
/// </summary>
public class InkExternalFunctions
{
    /// <summary>
    /// Binds external C# functions to be callable from Ink scripts
    /// </summary>
    /// <param name="story">The Ink story to bind functions to</param>
    public void Bind(Story story)
    {
        // Bind quest-related functions
        story.BindExternalFunction("StartQuest", (string questId) => StartQuest(questId));
        story.BindExternalFunction("AdvanceQuest", (string questId) => AdvanceQuest(questId));
        story.BindExternalFunction("CallFinishQuest", (string questId) => CallFinishQuest(questId));
        story.BindExternalFunction("FinishQuest", (string questId) => FinishQuest(questId));
        
        // Bind time and scene-related functions
        story.BindExternalFunction("SleepingEvent", (int value) => SleepingEvent(value));
        story.BindExternalFunction("SceneEvent", (string sceneId) => SceneEvent(sceneId));
    }

    /// <summary>
    /// Unbinds external functions when they're no longer needed
    /// </summary>
    /// <param name="story">The Ink story to unbind functions from</param>
    public void Unbind(Story story)
    {
        // Unbind all functions to prevent memory leaks
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("CallFinishQuest");
        story.UnbindExternalFunction("FinishQuest");
        story.UnbindExternalFunction("SleepingEvent");
        story.UnbindExternalFunction("SceneEvent");
    }

    /// <summary>
    /// Starts a new quest in the game's quest system
    /// </summary>
    /// <param name="questId">ID of the quest to start</param>
    private void StartQuest(string questId)
    {
        GameEventsManager.instance.questEvents.StartQuest(questId);
    }

    /// <summary>
    /// Advances a quest to its next step
    /// </summary>
    /// <param name="questId">ID of the quest to advance</param>
    private void AdvanceQuest(string questId)
    {
        GameEventsManager.instance.questEvents.AdvanceQuest(questId);
    }
    
    /// <summary>
    /// Finishes a quest immediately
    /// </summary>
    /// <param name="questId">ID of the quest to finish</param>
    private void FinishQuest(string questId)
    {
        GameEventsManager.instance.questEvents.FinishQuest(questId);
    }

    /// <summary>
    /// Signals that a quest should finish after dialogue ends
    /// </summary>
    /// <param name="questId">ID of the quest to finish</param>
    private void CallFinishQuest(string questId)
    {
        GameEventsManager.instance.questEvents.CallFinishQuest(questId);
    }

    /// <summary>
    /// Triggers a sleeping/time progression event
    /// </summary>
    /// <param name="value">Value indicating how much time to advance</param>
    private void SleepingEvent(int value)
    {
        TimeManager.instance.SleepingEvent(value);
    }

    /// <summary>
    /// Triggers a scene change event
    /// </summary>
    /// <param name="sceneId">ID of the scene to change to</param>
    private void SceneEvent(string sceneId)
    {
        TimeManager.instance.SceneEvent(sceneId);
    }

    /* Legacy code commented out
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
