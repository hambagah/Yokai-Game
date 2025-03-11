using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;
using Ink.Runtime;
//using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;
    
    private Story story;

    private int currentChoiceIndex = -1;

    private bool dialoguePlaying = false;
    private bool claimRewards = false;
    private string id;
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    GameObject temp;

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    private void Awake()
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnDestroy()
    {
        inkExternalFunctions.Unbind(story);
    }

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

    private void QuestStateChange(Quest quest)
    {
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            (Ink.Runtime.Object) new StringValue(quest.state.ToString())
        );
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void CallFinishQuest(string id)
    {
        claimRewards = true;
        this.id = id;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!inputEventContext.Equals(InputEventContext.DIALOGUE))
        {
            return;
        }
        
        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName)
    {
        temp = null;
        if (dialoguePlaying)
        {
            return;
        }

        dialoguePlaying = true;

        GameEventsManager.instance.dialogueEvents.DialogueStarted();
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was an empty string.");
        }
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        ContinueOrExitStory();
    }
    
    //Object's EnterDialogue will trigger events
    private void ObjectDialogue(string knotName, GameObject gameObject)
    {
        temp = gameObject;
        if (dialoguePlaying)
        {
            return;
        }

        dialoguePlaying = true;

        GameEventsManager.instance.dialogueEvents.DialogueStarted();
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was an empty string.");
        }
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        //ObjectContinueOrExitStory(gameObject);
        ContinueOrExitStory();
    }

    private void ContinueOrExitStory()
    {
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            currentChoiceIndex = -1;
        }

        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            while (IsLineBlank(dialogueLine) && story.canContinue)
            {
                dialogueLine = story.Continue();
            }

            if (IsLineBlank(dialogueLine) && !story.canContinue)
            {
                ExitDialogue();
                Debug.Log("1");
            }
            else
            {
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
                Debug.Log("2");
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            ExitDialogue();
            Debug.Log("3");
        }
    }

    private void ExitDialogue()
    {
        if (temp != null)
        {
            ObjectPoint comp = temp.GetComponent<ObjectPoint>();
            if (comp.objectType == 1 || comp.objectType == 3) //Boxes
            {
                comp.Cleaned();
            }
        }

        dialoguePlaying = false;
        GameEventsManager.instance.dialogueEvents.DialogueFinished();
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);
        if (claimRewards)
            {
            GameEventsManager.instance.questEvents.FinishQuest(id);
            id = "";
            claimRewards = false;
        }
        inkDialogueVariables.StopListening(story);
        story.ResetState();
    }

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