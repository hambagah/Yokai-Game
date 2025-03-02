using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

    public class Player : MonoBehaviour
    {

        public float speed = 2f;
        private Vector2 running = Vector2.zero;
        private Vector2 velocity = Vector2.zero;
        //private PlayerControls playerControls;
        private Rigidbody rb;

        private bool movementDisabled = false;
        
        private void Awake() {
            //playerControls = new PlayerControls();
            rb = GetComponent<Rigidbody>();   
        }

        private void Start()
        {
            GameEventsManager.instance.inputEvents.onMovePressed += MovePressed;
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement += DisablePlayerMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement += EnablePlayerMovement;
        }

        private void OnDestroy()
        {
            GameEventsManager.instance.inputEvents.onMovePressed -= MovePressed;
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement -= DisablePlayerMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement -= EnablePlayerMovement;
        }

        private void DisablePlayerMovement()
        {
            movementDisabled = true;
            velocity = Vector2.zero;
        }

        private void EnablePlayerMovement()
        {
            movementDisabled = false;
        }

        private void MovePressed(Vector2 moveDir)
        {
            velocity = moveDir.normalized * 4;

            if (movementDisabled)
            {
                velocity = Vector2.zero;
            }
        }

        private void ShiftPressed(Vector2 moveDir)
        {
            running = moveDir.normalized * 4;

            if (movementDisabled)
            {
                running = Vector2.zero;
            }
        }

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
                shift = 0;*/

            /*if (InputManager.GetInstance().GetInteractPressed())
                Debug.Log("Interact");*/
            //zAxis = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
            //xAxis = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            //shift = (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);
        }

        private void FixedUpdate()
        {
            /*if (DialogueManager.GetInstance().dialogueIsPlaying)
            {
                return;
            }*/
            
            //HandleMovement();            
            //rb.velocity = new Vector3(velocity.x * (speed + (running.y * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), velocity.y * (speed + (running.y * 5f)));
            rb.velocity = new Vector3 (velocity.x, Mathf.Min(rb.velocity.y *1.25f, 0), velocity.y);

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
            /*Vector2 moving = InputManager.GetInstance().GetMoveDirection();
            Vector2 running = InputManager.GetInstance().GetShiftPressed();
            //Debug.Log(running);
            
            rb.velocity = new Vector3(moving.x * (speed + (running.y * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), moving.y * (speed + (running.y * 5f)));
            */
            //rb.velocity = new Vector3(moving.x * (speed + (shift * 5f)), Mathf.Min(rb.velocity.y*1.25f, 0), moving.y * (speed + (shift * 5f)));

            

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
