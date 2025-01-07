using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private float d1xOg, d1yOg, d1zOg;
    private float d2xOg, d2yOg, d2zOg;
    //where you want them to move
    public float d1xPos, d1yPos, d1zPos;
    public float d2xPos, d2yPos, d2zPos;
    private GameObject player;
    public bool open = false;
    //Ignore
    //public GameObject interactable;
    public GameObject door1, door2;
    void Start()
    {
        d1xOg = door1.transform.position.x;
        d1yOg = door1.transform.position.y;
        d1zOg = door1.transform.position.z;
        d2xOg = door2.transform.position.x;
        d2yOg = door2.transform.position.y;
        d2zOg = door2.transform.position.z;
        player = GameObject.Find("Player");
    }

    void Update()
    {
        OpenClose();
    }

    private void OpenClose(){
        if (Vector3.Distance(player.transform.position, new Vector3(d1xOg, d1yOg, d1zOg)) < 2 || Vector3.Distance(player.transform.position,new Vector3(d2xOg, d2yOg, d2zOg)) < 2)
        {
            //Debug.Log("Open");
            //interactable.SetActive(true);
            open = true;
            if (Vector3.Distance(door1.transform.position, new Vector3(d1xPos, d1yPos, d1zPos)) < 0.01f || Vector3.Distance(door2.transform.position, new Vector3(d2xPos, d2yPos, d2zPos)) < 0.01f) {
            }
            else {
                door1.transform.position = Vector3.MoveTowards(door1.transform.position, new Vector3(d1xPos, d1yPos, d1zPos), 0.025f);
                door2.transform.position = Vector3.MoveTowards(door2.transform.position, new Vector3(d2xPos, d2yPos, d2zPos), 0.025f);
            }
        }
        else //if (Vector3.Distance(player.transform.position, door1.transform.position) > 3 || Vector3.Distance(player.transform.position, door2.transform.position) > 3)
        {
            //Debug.Log("Close");
            //interactable.GetComponent<Image>().CrossFadeAlpha(0.1f, 2.0f, false);
            //When you get a proper button UI add this so you can fade them in and out.
            //interactable.SetActive(false);
            open = false;
            door1.transform.position = Vector3.MoveTowards(door1.transform.position, new Vector3(d1xOg, d1yOg, d1zOg), 0.04f);
            door2.transform.position = Vector3.MoveTowards(door2.transform.position, new Vector3(d2xOg, d2yOg, d2zOg), 0.04f);     
        }
    }
}
