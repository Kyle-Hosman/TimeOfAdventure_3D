using UnityEngine;
using UnityEngine.InputSystem;

// This script acts as a proxy for the PlayerInput component
// such that the input events the game needs to process will 
// be sent through the GameEventManager. This lets any other
// script in the project easily subscribe to an input action
// without having to deal with the PlayerInput component directly.

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            Vector2 inputDirection = context.ReadValue<Vector2>();
            Vector3 movementDirection = new Vector3(inputDirection.x, 0, inputDirection.y); // Map 2D input to 3D space
            GameEventsManager.instance.inputEvents.MovePressed(movementDirection);
        }
    }

    public void LookPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();
            GameEventsManager.instance.inputEvents.LookPressed(lookInput);
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

    public void InventoryTogglePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.InventoryTogglePressed();
        }
    }

    public void NavigateInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            GameEventsManager.instance.inputEvents.NavigateInventory(direction);
        }
    }

    public void SelectInventoryItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.SelectInventoryItem();
        }
    }

    public void JumpPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.JumpPressed();
        }
    }

    public void InteractPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.InteractPressed();
        }
    }

    public void SprintPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.SprintPressed();
        }
        else if (context.canceled)
        {
            GameEventsManager.instance.inputEvents.SprintReleased();
        }
    }

    public void AttackPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManager.instance.inputEvents.AttackPressed();
        }
    }
}