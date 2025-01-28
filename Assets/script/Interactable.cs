using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using At0m1c.DialogueSystem;

public class Interactable : MonoBehaviour
{
    public float radius;
    private bool active; //If active = true then if you get close it automatically triggers else you have to manually interact
    private GameObject player;
    [SerializeField] Canvas buttonCanvas;
    [SerializeField] DialogueScript dialogue;
    [SerializeField] Canvas dialogueCanvas;
    public int state;
    public int id;
    void Start()
    {
        player = GameObject.Find("Player");
        dialogue = gameObject.GetComponent<DialogueScript>();
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < radius) {
            buttonCanvas.enabled = true;
            if (Input.GetKeyDown(KeyCode.E) && !(dialogueCanvas.enabled))
            {
                dialogue.StartDialogue();
                /*
                if (gameObject.tag == "Object") {
                    switch (state)
                    {
                        case 0: //default
                            Debug.Log("Erm");
                            break;
                        case 1:
                            Debug.Log("Box"); 
                            //dialogue.StartDialogue(day, gameObject.name, line);
                            break;
                        case 2:
                            Debug.Log("Not a box");
                            break;
                    }
                }*/
            }
        }
        else buttonCanvas.enabled = false;
    }

    /*void Dialogue()
    {
        switch (state)
        {
            case 1:

        }
    }*/
}
