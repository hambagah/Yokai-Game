using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

    public class Player : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private bool loadPlayerState = true;
        [SerializeField] private Animator animator;

        public float speed = 2f;
        //private Vector2 running = Vector2.zero;
        private int running = 0;
        private Vector2 velocity = Vector2.zero;
        //private PlayerControls playerControls;
        private Rigidbody rb;
        [SerializeField] private GameObject rig;

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
                float Xpos = PlayerPrefs.GetFloat ("PlayerX");
                float Ypos = PlayerPrefs.GetFloat ("PlayerY");
                float Zpos = PlayerPrefs.GetFloat ("PlayerZ");
                Vector3 relocation = new Vector3(Xpos, Ypos, Zpos);
                if (transform.position == relocation)
                {
                    loadPlayerState = false;
                }
                this.gameObject.transform.position = relocation;
                Debug.Log("Loaded: " + relocation);
            }
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector3 (velocity.x, Mathf.Min(rb.velocity.y *1.25f, 0), velocity.y);
            
            if (rb.velocity.x > 0 && rb.velocity.z > 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 45, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.x < 0 && rb.velocity.z > 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 315, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.x < 0 && rb.velocity.z < 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 225, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.x > 0 && rb.velocity.z < 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 135, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.z > 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 0, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.z < 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 180, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.x > 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 90, 0);
                animator.SetFloat("forward", 1);
            }
            else if (rb.velocity.x < 0)
            {
                rig.transform.eulerAngles = new Vector3(0, 270, 0);
                animator.SetFloat("forward", 1);
            }
            else {
                animator.SetFloat("forward", 0);
            }
            

            if (velocity == Vector2.zero)
            {
                running = 0;
            }
        }

    private void OnApplicationQuit()
    {
        SavePlayer(gameObject.transform.position);
    }

    public void SavePlayer(Vector3 playerPosition)
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
            if (transform.position == relocation)
            {
                loadPlayerState = false;
            }
            this.gameObject.transform.position = relocation;
            Debug.Log("Loaded: " + relocation);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load player position ");
        }
    }
}
    
