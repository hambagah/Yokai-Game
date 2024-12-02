using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool playerTurn = false;
    public float timer;
    private float endTurnTime;
    private TMP_Text timerText;

    void Start()
    {
        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer-= Time.deltaTime;
            timerText.text = timer.ToString("#.");
            playerTurn = true;
        }
        else
        {
            playerTurn = false;
        }

        //Enemy calculating turn 
        //Simple timer function rn
        if (!playerTurn)
        {
            timer = 30;
        }
    }
}
