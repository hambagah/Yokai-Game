using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using At0m1c.DialogueSystem;

public class Interactable : MonoBehaviour
{
    //private bool active; //If active = true then if you get close it automatically triggers else you have to manually interact
    //private GameObject player;
    /*[SerializeField] DialogueScript dialogue;
    [SerializeField] DialogueObject dialogueObject;
    [SerializeField] Canvas dialogueCanvas;

    public GameObject gameStatus;

    public int state;
    public int id;*/

    //[SerializeField] private GameObject visualCue;
    [SerializeField] private GameObject visualCue;
    private bool playerInRange;

    [SerializeField] private Animator emoteAnimator;

    [SerializeField] private TextAsset inkJSON;

    [SerializeField] private int objectType;
    
    /*void Start()
    {
        player = GameObject.Find("Player");
        dialogue = gameObject.GetComponent<DialogueScript>();
        gameStatus = GameObject.Find("GameState");
    }*/

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    void Update()
    {
        /*if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            visualCue.SetActive(true);
            /*if (InputManager.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON, emoteAnimator, objectType, gameObject);
            }/
        }
        else {
            visualCue.SetActive(false);
        }*/
    }

    public void Cleaned()
    {
        GameEventsManager.instance.miscEvents.ObjectCleaned();
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
