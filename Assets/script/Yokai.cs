using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yokai : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Jump())
        {
            transform.position = new Vector3(0, 1.45f, 1);
        }
        else
        {
            transform.position = new Vector3(0, 1.33f, 1);
        }*/
    }

    public void Jump()
    {
        transform.position = transform.position + new Vector3(0, 0.03f, 0);
        //return true;
    }

    public void UnJump()
    {
        transform.position = transform.position + new Vector3(0, -0.03f, 0);
        //return true;
    }

    public void Attack()
    {
        Debug.Log("ATTACKING");
    }
}
