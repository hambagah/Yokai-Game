using UnityEngine;
using UnityEngine.InputSystem;

// This script acts as a single point for all other scripts to get
// the current input from. It uses Unity's new Input System and
// functions should be mapped to their corresponding controls
// using a PlayerInput component with Unity Events.

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            GameEventsManager.instance.inputEvents.MovePressed(context.ReadValue<Vector2>());
        }
    }

    public void ShiftPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            //GameEventsManager.instance.inputEvents.ShiftPressed(context.ReadValue<Vector2>());
            GameEventsManager.instance.inputEvents.ShiftPressed();
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.SubmitPressed();
        }
    }

    public void QuestLogTogglePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.QuestLogTogglePressed();
        }
    }

    /*
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 shiftPressed = Vector2.zero;
    private bool interactPressed = false;

    private static InputManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static InputManager GetInstance() 
    {
        return instance;
    }

    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveDirection = context.ReadValue<Vector2>();
        } 
    }

    public void ShiftPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //shiftPressed = true;
            shiftPressed = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            //shiftPressed = false;
            shiftPressed = context.ReadValue<Vector2>();
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        } 
    }

    public Vector2 GetMoveDirection() 
    {
        return moveDirection;
    }

    // for any of the below 'Get' methods, if we're getting it then we're also using it,
    // which means we should set it to false so that it can't be used again until actually
    // pressed again.

    public Vector2 GetShiftPressed()
    {
        return shiftPressed;
    }

    public bool GetInteractPressed() 
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public void RegisterInteractPressed() 
    {
        interactPressed = false;
    }*/

}