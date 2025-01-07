using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 2f;

    private float zAxis, xAxis, shift;
    private Rigidbody rb;
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        zAxis = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
        xAxis = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        shift = (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(xAxis, 0, zAxis);
        //rb.MovePosition(transform.position + movement * Time.deltaTime * speed);   
        //Debug.Log(zAxis + " " + xAxis);
        
        rb.velocity = movement * (speed + (shift * 5f));
        
        //rb.AddForce(movement*speed, ForceMode.VelocityChange);
        //rb.AddForce(transform.position + movement * speed, ForceMode.Impulse);
    }
}
