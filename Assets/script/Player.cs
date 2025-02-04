using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace At0m1c.DialogueSystem {
    public class Player : MonoBehaviour
    {

        public float speed = 2f;
        //private PlayerControls playerControls;
        private Vector2 move;
        private float shift;
        private Rigidbody rb;
        
        private void Awake() {
            //playerControls = new PlayerControls();
            rb = gameObject.GetComponent<Rigidbody>();   
        }

        /*private void OnEnable() {
            playerControls.Enable();
        }

        private void OnDisable() {
            playerControls.Disable();
        }*/

        // Update is called once per frame
        void Update()
        {
            //move = playerControls.Player.WASD.ReadValue<Vector2>();
            //Debug.Log(move);
            /*if (playerControls.Player.Run.triggered) {
                shift = 1;
                Debug.Log("Running");
            }
            else 
                shift = 0;
            if (playerControls.Player.Interact.triggered) {
                Debug.Log("Interact");
            }*/
            /*if (InputManager.GetInstance().GetShiftPressed()) {
                shift = 1;
                Debug.Log("Running");
            }
            else 
                shift = 0;

            if (InputManager.GetInstance().GetInteractPressed())
                Debug.Log("Interact");*/
            //zAxis = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
            //xAxis = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            //shift = (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);
        }

        private void FixedUpdate()
        {
            if (DialogueManager.GetInstance().dialogueIsPlaying)
            {
                return;
            }
            
            HandleMovement();
            //Vector3 movement = new Vector3(xAxis, rb.velocity.y, zAxis);
            //rb.MovePosition(transform.position + movement * Time.deltaTime * speed);   
            //Debug.Log(zAxis + " " + xAxis);
            
            //rb.velocity = movement * (speed + (shift * 5f));
            //rb.velocity = new Vector3(move.x * (speed + (shift * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), move.z * (speed + (shift * 5f)));
            //Vector2 moving = playerControls.Player.WASD.ReadValue<Vector2>();
            //Debug.Log(moving + "Player Script");
            //rb.velocity = new Vector3(move.x * (speed + (shift * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), move.y * (speed + (shift * 5f)));
        }

        private void HandleMovement()
        {
            Vector2 moving = InputManager.GetInstance().GetMoveDirection();
            rb.velocity = new Vector3(moving.x * (speed + (shift * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), moving.y * (speed + (shift * 5f)));

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