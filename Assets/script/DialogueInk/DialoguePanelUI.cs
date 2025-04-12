using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

/// <summary>
/// Controls the visual representation of dialogue in the UI.
/// Handles displaying dialogue text and choice buttons.
/// </summary>
public class DialoguePanelUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;          // Parent container for all dialogue UI elements
    [SerializeField] private TextMeshProUGUI dialogueText;      // Text component for displaying dialogue content
    [SerializeField] private DialogueChoiceButton[] choiceButtons; // Array of choice buttons

    /// <summary>
    /// Initialize the dialogue panel in a hidden state
    /// </summary>
    private void Awake()
    {
        contentParent.SetActive(false);
        ResetPanel();
    }

    /// <summary>
    /// Subscribe to dialogue events when enabled
    /// </summary>
    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;
    }
    
    /// <summary>
    /// Unsubscribe from dialogue events when disabled
    /// </summary>
    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue -= DisplayDialogue;
    }

    /// <summary>
    /// Show the dialogue panel when dialogue starts
    /// </summary>
    private void DialogueStarted()
    {
        contentParent.SetActive(true);
    }

    /// <summary>
    /// Hide and reset the dialogue panel when dialogue ends
    /// </summary>
    private void DialogueFinished()
    {
        contentParent.SetActive(false);
        ResetPanel();
    }

    /// <summary>
    /// Display dialogue text and available choices
    /// </summary>
    /// <param name="dialogueLine">The line of dialogue to display</param>
    /// <param name="dialogueChoices">List of available choices from the Ink story</param>
    private void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices)
    {
        // Set dialogue text
        dialogueText.text = dialogueLine;

        // Validate choice count
        if (dialogueChoices.Count > choiceButtons.Length)
        {
            Debug.LogError("More dialogue choices (" + dialogueChoices.Count + ") came through than are supported ("
            + choiceButtons.Length + ").");
        }

        // Hide all choice buttons initially
        foreach (DialogueChoiceButton choiceButton in choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
        }

        // Display choices in reverse order (bottom to top)
        int choiceButtonIndex = dialogueChoices.Count - 1;
        for (int inkChoiceIndex = 0; inkChoiceIndex < dialogueChoices.Count; inkChoiceIndex++)
        {
            Choice dialogueChoice = dialogueChoices[inkChoiceIndex];
            DialogueChoiceButton choiceButton = choiceButtons[choiceButtonIndex];

            // Configure the choice button
            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceText(dialogueChoice.text);
            choiceButton.SetChoiceIndex(inkChoiceIndex);

            // Auto-select the first choice
            if (inkChoiceIndex == 0)
            {
                choiceButton.SelectButton();
                GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(0);
            }

            choiceButtonIndex--;
        }
    }

    /// <summary>
    /// Reset the panel to its default state
    /// </summary>
    private void ResetPanel()
    {
        dialogueText.text = "";
    }
}
