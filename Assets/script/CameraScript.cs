using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float zoomAmount = 0; //With Positive and negative values
    float maxToClamp = 7;
    float minToClamp = 0;
    //float rotSpeed = 10;
    Camera cam;
    float cameraSize;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        cameraSize = cam.orthographicSize;
    }

    void Update() 
    {
        zoomAmount += Input.GetAxis("Mouse ScrollWheel");
        zoomAmount = Mathf.Clamp(zoomAmount, minToClamp, maxToClamp);
        //var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), maxToClamp - Mathf.Abs(zoomAmount));
        //gameObject.transform.Translate(0,0,translate * rotSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));
        cam.orthographicSize = cameraSize - zoomAmount;
    }
}
