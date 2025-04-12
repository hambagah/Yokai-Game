using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Core manager for the dialogue system.
/// Handles the Ink story, processes dialogue flow, and manages dialogue state transitions.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;           // The Ink story JSON asset
    
    private Story story;                                  // The Ink runtime story instance
    private int currentChoiceIndex = -1;                  // Currently selected choice
    private bool dialoguePlaying = false;                 // Whether dialogue is currently active
    private bool claimRewards = false;                    // Whether to claim quest rewards when dialogue ends
    private string id;                                    // ID for quest completion
    
    // Ink story tags
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private GameObject temp;                             // Temporary reference to interacted object

    private InkExternalFunctions inkExternalFunctions;   // Handles external function binding
    private InkDialogueVariables inkDialogueVariables;   // Manages dialogue variables

    /// <summary>
    /// Initialize the Ink story and supporting systems
    /// </summary>
    private void Awake()
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    /// <summary>
    /// Clean up when component is destroyed
    /// </summary>
    private void OnDestroy()
    {
        inkExternalFunctions.Unbind(story);
    }

    /// <summary>
    /// Subscribe to events when component is enabled
    /// </summary>
    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.dialogueEvents.onObjectDialogue += ObjectDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.dialogueEvents.onCallFinishQuest += CallFinishQuest;
    }

    /// <summary>
    /// Unsubscribe from events when component is disabled
    /// </summary>
    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.dialogueEvents.onObjectDialogue -= ObjectDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.dialogueEvents.onCallFinishQuest -= CallFinishQuest;
        //GameEventsManager.instance.questEvents.onFinishQuest -= onFinishQuest;
    }

    /// <summary>
    /// Update quest state variables in the Ink story when quest state changes
    /// </summary>
    /// <param name="quest">The quest that changed state</param>
    private void QuestStateChange(Quest quest)
    {
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            (Ink.Runtime.Object) new StringValue(quest.state.ToString())
        );
    }

    /// <summary>
    /// Update an Ink dialogue variable
    /// </summary>
    /// <param name="name">Variable name</param>
    /// <param name="value">New variable value</param>
    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    /// <summary>
    /// Update the currently selected choice index
    /// </summary>
    /// <param name="choiceIndex">New choice index</param>
    private void UpdateChoiceIndex(int choiceIndex)
    {
        this.currentChoiceIndex = choiceIndex;
    }

    /// <summary>
    /// Set up a quest to complete when dialogue ends
    /// </summary>
    /// <param name="id">ID of the quest to finish</param>
    private void CallFinishQuest(string id)
    {
        claimRewards = true;
        this.id = id;
    }

    /// <summary>
    /// Process submit input during dialogue
    /// </summary>
    /// <param name="inputEventContext">Context of the input event</param>
    private void SubmitPressed(InputEventContext inputEventContext)
    {
        // Only process input in dialogue context
        if (!inputEventContext.Equals(InputEventContext.DIALOGUE))
        {
            return;
        }
        
        ContinueOrExitStory();
    }

    /// <summary>
    /// Enter dialogue mode with the specified story knot
    /// </summary>
    /// <param name="knotName">The Ink knot to start from</param>
    private void EnterDialogue(string knotName)
    {
        temp = null;
        if (dialoguePlaying)
        {
            return;
        }

        // Set up dialogue state
        dialoguePlaying = true;

        // Notify systems about dialogue start
        GameEventsManager.instance.dialogueEvents.DialogueStarted();
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        // Jump to the specified knot in the story
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was an empty string.");
        }
        
        // Sync variables with the Ink story
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        ContinueOrExitStory();
    }
    
    /// <summary>
    /// Enter dialogue mode with an interactive object
    /// </summary>
    /// <param name="knotName">The Ink knot to start from</param>
    /// <param name="gameObject">The object being interacted with</param>
    private void ObjectDialogue(string knotName, GameObject gameObject)
    {
        temp = gameObject;
        if (dialoguePlaying)
        {
            return;
        }

        // Set up dialogue state
        dialoguePlaying = true;

        // Notify systems about dialogue start
        GameEventsManager.instance.dialogueEvents.DialogueStarted();
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        // Jump to the specified knot in the story
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was an empty string.");
        }
        
        // Sync variables with the Ink story
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        //ObjectContinueOrExitStory(gameObject);
        ContinueOrExitStory();
    }

    /// <summary>
    /// Continue the story or exit if at the end
    /// </summary>
    private void ContinueOrExitStory()
    {
        // Process choice selection if a choice is selected
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            // Don't invoke the button's onClick - it causes a circular reference
            // and stack overflow with DialogueChoiceButton.OnButtonClicked
            
            // Proceed with Ink choice selection
            story.ChooseChoiceIndex(currentChoiceIndex);
            currentChoiceIndex = -1;
        }

        // Continue the story if possible
        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            // Skip blank lines
            while (IsLineBlank(dialogueLine) && story.canContinue)
            {
                dialogueLine = story.Continue();
            }

            // Exit if at end of content
            if (IsLineBlank(dialogueLine) && !story.canContinue)
            {
                ExitDialogue();
                Debug.Log("1");
            }
            else
            {
                // Display the current dialogue line and choices
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
                Debug.Log("2");
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            // No choices and can't continue means end of dialogue
            ExitDialogue();
            Debug.Log("3");
        }
    }

    /// <summary>
    /// Exit dialogue mode and clean up
    /// </summary>
    private void ExitDialogue()
    {
        // Handle object cleanup if interacting with an object
        if (temp != null)
        {
            ObjectPoint comp = temp.GetComponent<ObjectPoint>();
            if (comp.objectType == 1 || comp.objectType == 3) // Boxes
            {
                comp.Cleaned();
            }
        }

        // Reset dialogue state
        dialoguePlaying = false;
        
        // Notify systems about dialogue end
        GameEventsManager.instance.dialogueEvents.DialogueFinished();
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);
        
        // Complete quest if needed
        if (claimRewards)
        {
            GameEventsManager.instance.questEvents.FinishQuest(id);
            id = "";
            claimRewards = false;
        }
        
        // Clean up Ink story state
        inkDialogueVariables.StopListening(story);
        story.ResetState();
    }

    /// <summary>
    /// Check if a dialogue line is blank (empty or just whitespace)
    /// </summary>
    /// <param name="dialogueLine">Line to check</param>
    /// <returns>True if the line is blank</returns>
    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }

    /*private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Error with tag: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    break;
            }
        }
    }*/

    /*//Dialogue UI
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon; 
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private TextAsset loadGlobalsJSON;
    private Coroutine displayLineCoroutine;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;

    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;


    //Choices UI
    [SerializeField] private GameObject[] choices;

    private TextMeshProUGUI[] choicesText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueToNextLine = false;

    private static DialogueManager instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private DialogueVariables dialogueVariables;

    private InkExternalFunctions inkExternalFunctions;

    private GameObject passedObject;
    private int objectId;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Dialogue Manager in scene.");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
        inkExternalFunctions = new InkExternalFunctions();
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    public void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        layoutAnimator = dialoguePanel.GetComponent<Animator>();

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if(!dialogueIsPlaying)
        {
            return;
        }

        /*if (canContinueToNextLine && currentStory.currentChoices.Count == 0 && InputManager.GetInstance().GetInteractPressed())
        {
            ContinueStory();
        }/
    }

    public void EnterDialogueMode(TextAsset inkJSON, Animator emoteAnimator, int objectType, GameObject objectPass)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);
        inkExternalFunctions.Bind(currentStory, emoteAnimator);
        passedObject = objectPass;
        objectId = objectType;

        //Reset UI
        displayNameText.text = "";
        portraitAnimator.Play("default");
        layoutAnimator.Play("left");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);
        inkExternalFunctions.Unbind(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        if (objectId == 1) //Destroy object after use 
        {
            passedObject.GetComponent<Interactable>().Cleaned();
        }
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            //dialogueText.text = currentStory.Continue();
            string nextLine = currentStory.Continue();

            if (nextLine.Equals("") && !currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }
            else
            {
                HandleTags(currentStory.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        foreach (char letter in line.ToCharArray())
        {
            /*if (InputManager.GetInstance().GetInteractPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }/

            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }

        }

        continueIcon.SetActive(true);
        DisplayChoices();
        canContinueToNextLine = true;

    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Error with tag: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Too many choices");
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine) {
            currentStory.ChooseChoiceIndex(choiceIndex);
            //Fix for their bug Idk if this affects the current one but better safe than sorry
            //InputManager.GetInstance().RegisterInteractPressed();
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Var null " + variableName);
        }
        return variableValue;
    }

    //save on exit might need to change
    public void OnApplicationQuit()
    {
        if (dialogueVariables != null) {
            dialogueVariables.SaveVariables();
        }
    }*/
}