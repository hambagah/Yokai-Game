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
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < radius) {
            interactable.SetActive(true);
            if (Input.GetKey(KeyCode.E)) 
            {
                SceneManager.LoadScene(1);
            }
        }
        else interactable.SetActive(false);
    }
}
