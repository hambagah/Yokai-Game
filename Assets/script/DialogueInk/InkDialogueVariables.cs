using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

/// <summary>
/// Manages variables between Unity and Ink narrative engine.
/// Tracks, syncs, and updates variables to maintain consistency between gameplay and dialogue.
/// </summary>
public class InkDialogueVariables
{
    private Dictionary<string, Ink.Runtime.Object> variables; // Dictionary to store all dialogue variables

    /// <summary>
    /// Initializes dialogue variables from an Ink story
    /// </summary>
    /// <param name="story">The Ink story to initialize variables from</param>
    public InkDialogueVariables(Story story)
    {
        variables = new Dictionary<string, Ink.Runtime.Object>();
        
        // Initialize variables from the story
        foreach (string name in story.variablesState)
        {
            Ink.Runtime.Object value = story.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    /// <summary>
    /// Syncs Unity variables to the Ink story and starts listening for changes
    /// </summary>
    /// <param name="story">The Ink story to sync with</param>
    public void SyncVariablesAndStartListening(Story story)
    {
        SyncVariablesToStory(story);
        story.variablesState.variableChangedEvent += UpdateVariableState;
    }

    /// <summary>
    /// Stops listening for variable changes in the Ink story
    /// </summary>
    /// <param name="story">The Ink story to stop listening to</param>
    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= UpdateVariableState;
    }

    /// <summary>
    /// Updates a variable's value when it changes in the Ink story
    /// </summary>
    /// <param name="name">Name of the variable</param>
    /// <param name="value">New value of the variable</param>
    public void UpdateVariableState(string name, Ink.Runtime.Object value)
    {
        // Skip if variable doesn't exist in our dictionary
        if (!variables.ContainsKey(name))
        {
            return;
        }
        
        // Update the variable value
        variables[name] = value;
        Debug.Log("Updated dialogue variable: " + name + " = " + value);
    }

    /// <summary>
    /// Syncs all tracked variables to the Ink story
    /// </summary>
    /// <param name="story">The Ink story to sync variables to</param>
    private void SyncVariablesToStory(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
}
