using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a dialogue choice button in the UI.
/// Handles selection events and communicates with the dialogue system.
/// </summary>
public class DialogueChoiceButton : MonoBehaviour, ISelectHandler
{
    [Header("Components")]
    [SerializeField] private Button button;              // Button component for selection and interaction
    [SerializeField] private TextMeshProUGUI choiceText; // Text component for displaying choice text

    private int choiceIndex = -1;                        // Index of this choice in the current dialogue choices

    /// <summary>
    /// Set up button events on initialization
    /// </summary>
    private void Awake()
    {
        // Ensure the button's onClick is properly set up
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// Handles button click events
    /// </summary>
    public void OnButtonClicked()
    {
        // Just update the choice index - don't call SubmitPressed to avoid infinite loop
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
        // Don't call SubmitPressed here - it causes a stack overflow
    }

    /// <summary>
    /// Sets the text content of this choice button
    /// </summary>
    /// <param name="choiceTextString">The text to display</param>
    public void SetChoiceText(string choiceTextString)
    {
        choiceText.text = choiceTextString;
    }

    /// <summary>
    /// Sets the index of this choice in relation to the Ink story choices
    /// </summary>
    /// <param name="choiceIndex">Zero-based index of the choice</param>
    public void SetChoiceIndex(int choiceIndex)
    {
        this.choiceIndex = choiceIndex;
    }

    /// <summary>
    /// Selects this button in the UI navigation system
    /// </summary>
    public void SelectButton()
    {
        button.Select();
    }

    /// <summary>
    /// Called when this button becomes selected in the UI system.
    /// Updates the current choice index in the dialogue system.
    /// </summary>
    /// <param name="eventData">Event data from the UI system</param>
    public void OnSelect(BaseEventData eventData)
    {
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(choiceIndex);
    }
}
