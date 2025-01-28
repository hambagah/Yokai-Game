using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform origin = null;
    public GameObject StreamPrefab = null;

    private bool isPouring = false;
    private Stream currentStream = null;

    private void Update()
    {
        bool pourCheck = CalculatePourAngel() > pourThreshold;

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;
            if (isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }

    }

    private void StartPour()
    {
        Debug.Log("Pouring started");
        currentStream = CreateStream();
        if (currentStream != null)
        {
            currentStream.Begin();
        }
    }

    private void EndPour()
    {
        Debug.Log("Pouring ended");
        if (currentStream != null)
        {
            currentStream.End();
            currentStream = null;
        }
    }

    private float CalculatePourAngel()
    {
        Vector3 pourDirection = -transform.up; // Local downward direction
        return Vector3.Angle(Vector3.down, pourDirection);
    }

    private Stream CreateStream()
    {
        GameObject streamObject = Instantiate(StreamPrefab, origin.position, Quaternion.identity, transform);

        return streamObject.GetComponent<Stream>();

    }
}