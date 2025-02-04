/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//
//This script is outdated
//Probably remove it.
//
//
//

namespace At0m1c.DialogueSystem {
public class DialogueScript : MonoBehaviour
{
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject dialogueOptionsContainer;
    [SerializeField] Transform dialogueOptionsParent;
    [SerializeField] GameObject dialogueOptionsButtonPrefab;
    [SerializeField] DialogueObject startDialogueObject;
    
    private GameObject gameState;
    //private Timer time;

    bool optionSelected = false;
    private int id;
    private bool trigger = false;

    private Interactable interact; 

    void Start()
    {
        //textComponent.text = string.Empty;
        gameState = gameObject.GetComponent<Interactable>().gameStatus;
        //time = gameObject.GetComponent<Interactable>().timer;
        id = gameObject.GetComponent<Interactable>().id;
    }

    void Update()
    {
        /*if(Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }

        if (textComponent.text == lines[index])
        {
            //Button.SetActive(true); for any confirm buttons
        }*
    }

    public void StartDialogue()
    {
        StartCoroutine(TypeLine(startDialogueObject));
    }

    public void StartDialogue(DialogueObject _dialogueObject)
    {
        StartCoroutine(TypeLine(_dialogueObject));
        trigger = true;
    }
    
    public void OptionSelected(DialogueObject selectedOption)
    {
        optionSelected = true;
        StartDialogue(selectedOption);
    }

    IEnumerator TypeLine(DialogueObject _dialogueObject)
    {
        /*foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }*
        yield return null;
        List<GameObject> spawnedButtons = new List<GameObject> ();

        dialogueCanvas.enabled = true;
        foreach (var dialogue in _dialogueObject.dialogueSegments)
        {
            dialogueText.text = dialogue.dialogueText;
            if (dialogue.dialogueChoices.Count == 0) { //Auto run dialogue. Might replace with click 
                yield return new WaitForSeconds (dialogue.dialogueDisplayTime);
            }
            else { //Grabs the options button for dialogue
                dialogueOptionsContainer.SetActive(true);
                foreach (var option in dialogue.dialogueChoices) {
                    GameObject newButton = Instantiate (dialogueOptionsButtonPrefab, dialogueOptionsParent);
                    spawnedButtons.Add(newButton);
                    newButton.GetComponent<UIDialogueOption>().Setup(this, option.followOnDialogue, option.dialogueChoice);
                }

                while (!optionSelected) {
                    yield return null;
                }
                break;
            }
        }
        dialogueOptionsContainer.SetActive(false);
        dialogueCanvas.enabled = false;
        optionSelected = false;
        if (id == 0) { //One time interact objects have ID of 0
            TimeManager.GetInstance().UpdateTime(3);
            Destroy(gameObject); 
        }

        if (id == 1) {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        if (id == 2 && trigger) {
            TimeManager.GetInstance().ResetDay();
        }
        
        /*if (interact.GetComponent<Interactable>().id == 2) { //Conditional objects EX: Bed can only be interacted during
            interact.GetComponent<Timer>().ResetDay();
        }*

        spawnedButtons.ForEach(x => Destroy(x));
    }

    void NextLine()
    {
        /*if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            //if (needs to open scene) {} else ignore
            //SceneManager.LoadScene(1);
        }*
    }
}
}*/