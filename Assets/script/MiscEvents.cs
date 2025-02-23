using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscEvents 
{
    public event Action objectCleaned;

    public void ObjectCleaned()
    {
        if (objectCleaned != null)
        {
            objectCleaned();
            Debug.Log("Cleaned");
        }
    }
}
