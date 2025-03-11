using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

    public class Player : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private bool loadPlayerState = true;

        public float speed = 2f;
        //private Vector2 running = Vector2.zero;
        private int running = 0;
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
            GameEventsManager.instance.inputEvents.onShiftPressed += ShiftPressed;
            GameEventsManager.instance.playerEvents.onDisablePlayerMovement += DisablePlayerMovement;
            GameEventsManager.instance.playerEvents.onEnablePlayerMovement += EnablePlayerMovement;
        }

        private void OnDestroy()
        {
            GameEventsManager.instance.inputEvents.onMovePressed -= MovePressed;
            GameEventsManager.instance.inputEvents.onShiftPressed -= ShiftPressed;
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
            velocity = moveDir.normalized * (4 + running);

            if (movementDisabled)
            {
                velocity = Vector2.zero;
            }
        }

        private void ShiftPressed()
        {
            running = 4;
            /*if (running == 0)
            {
                running = 4;
            }
            else if (running == 4){
                running = 0;
            }*/
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
            rb.velocity = new Vector3 (velocity.x, Mathf.Min(rb.velocity.y *1.25f, 0), velocity.y);
            if (velocity == Vector2.zero)
            {
                running = 0;
            }
        }

    /*private void OnApplicationQuit()
    {
        SavePlayer(player.transform.pos);
    }

    private void SavePlayer(Vector3 playerPosition)
    {
        try 
        {
            PlayerPrefs.SetFloat("PlayerX", playerPosition.x);
            PlayerPrefs.SetFloat("PlayerY", playerPosition.y);
            PlayerPrefs.SetFloat("PlayerZ", playerPosition.z);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save player position " + playerPosition);
        }
    }

    private void LoadPlayer()
    {
        Quest quest = null;
        try 
        {
            float Xpos = PlayerPrefs.GetFloat ("PlayerX");
            float Ypos = PlayerPrefs.GetFloat ("PlayerY");
            float Zpos = PlayerPrefs.GetFloat ("PlayerZ");
            /*PlayerPrefsJTA.SetTransform("PlayerTransform", player.transform)
            // load quest from saved data
            if (PlayerPrefs.HasKey("0") && loadPlayerState)
            {
                string serializedData = PlayerPrefs.GetString(0);
                Vector3 playerPosition = JsonUtility.FromJson<Vector3>(serializedData);
                player.transform = playerPosition;
            }
            // otherwise, initialize a new quest
            /*else 
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save player position " + playerPosition);
        }
        this.gameObject.transform.position*/
    }
    
