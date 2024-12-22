using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public Door doorOpen; 
    public string stageToPlay;

    void Start()
    {
        //doorOpen = gameObject.GetComponent<Door>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && doorOpen.open)
        {
            SceneManager.LoadScene(stageToPlay, LoadSceneMode.Single);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        /*if (col.CompareTag("Player"))
        {
            SceneManager.LoadScene(stageToPlay, LoadSceneMode.Single);
        }*/
    }
}
