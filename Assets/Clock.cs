using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{   
    public float currTimer;
    public float maxTimer;
    public float startTimer;

    void Start()
    {
        currTimer = startTimer;
    }

    void Update()
    {
        currTimer += Time.deltaTime;
        if (currTimer > maxTimer) 
            currTimer = 0;
    }
}
