using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace At0m1c.DialogueSystem {
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
            zAxis = (Input.GetKey(KeyCode.X) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
            xAxis = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            shift = (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);
        }

        private void FixedUpdate()
        {
            //Vector3 movement = new Vector3(xAxis, rb.velocity.y, zAxis);
            //rb.MovePosition(transform.position + movement * Time.deltaTime * speed);   
            //Debug.Log(zAxis + " " + xAxis);
            
            //rb.velocity = movement * (speed + (shift * 5f));
            rb.velocity = new Vector3(xAxis * (speed + (shift * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), zAxis * (speed + (shift * 5f)));
            
            //rb.AddForce(movement*speed, ForceMode.VelocityChange);
            //rb.AddForce(transform.position + movement * speed, ForceMode.Impulse);
        }

        /*void OnTriggerEnter (Collider other) {
            if (other.CompareTag ("Player")) {
                if (OnEnterInteractable != null) OnEnterInteractable();
            }
        }

        void OnTriggerExit (Collider other) {
            if (other.CompareTag ("Player")) {
                if (OnExitInteractable != null) OnExitInteractable();
            }
        }*/
    }
}