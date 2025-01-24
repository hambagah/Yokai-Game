using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public float radius;
    private bool active; //If active = true then if you get close it automatically triggers else you have to manually interact
    private GameObject player;
    public GameObject interactable;
    public GameObject dialogue;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < radius) {
            interactable.SetActive(true);
            if (Input.GetKey(KeyCode.E)) 
            {
                dialogue.SetActive(true);
                //interactable needs the identity of the object that it's on. 
                //next it needs to identity which dialogue script it uses.
                //dialogue needs to be able to trigger scenes.
                //SceneManager.LoadScene(1);
            }
        }
        else interactable.SetActive(false);
    }
}
