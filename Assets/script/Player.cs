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
        }

        void Update()
        {
            if (loadPlayerState)
            {
                LoadPlayer();
            }
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector3 (velocity.x, Mathf.Min(rb.velocity.y *1.25f, 0), velocity.y);
            if (velocity == Vector2.zero)
            {
                running = 0;
            }
        }

    private void OnApplicationQuit()
    {
        SavePlayer(gameObject.transform.position);
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
        try 
        {
            float Xpos = PlayerPrefs.GetFloat ("PlayerX");
            float Ypos = PlayerPrefs.GetFloat ("PlayerY");
            float Zpos = PlayerPrefs.GetFloat ("PlayerZ");
            Vector3 relocation = new Vector3(Xpos, Ypos, Zpos);
            this.gameObject.transform.position = relocation;
            if (transform.position == relocation)
            {
                loadPlayerState = false;
            }
            Debug.Log("Loaded: " + relocation);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load player position ");
        }
    }
}
    
