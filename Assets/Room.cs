using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] walls;
    public Player player;
    int cameraAngle;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //if (player.Distance())
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < walls.Length; i++)
            {
                //walls[i].GetComponent<Renderer>().enabled = false;
                //walls[i].GetComponent<Renderer>().material.color.a = 0.1f;
                Color color = walls[i].GetComponent<Renderer>().material.color;
                color.a = 0.1f;
                walls[i].GetComponent<Renderer>().material.color = color;
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < walls.Length; i++)
            {
                //walls[i].GetComponent<Renderer>().enabled = true;
                //walls[i].GetComponent<Renderer>().material.color.a = 1f;
                Color color = walls[i].GetComponent<Renderer>().material.color;
                color.a = 1f;
                walls[i].GetComponent<Renderer>().material.color = color;
            }
        }
    }
}
