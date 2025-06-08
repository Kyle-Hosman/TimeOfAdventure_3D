using UnityEngine;
using System;

public class InputEvents
{
    public InputEventContext inputEventContext { get; private set; } = InputEventContext.DEFAULT;

    public void ChangeInputEventContext(InputEventContext newContext)
    {
        Debug.Log($"Changing InputEventContext from {inputEventContext} to {newContext}");
        this.inputEventContext = newContext;
    }

    // Movement Input
    public event Action<Vector3> onMovePressed; // Updated to use Vector3 for 3D movement
    public void MovePressed(Vector3 moveDir)
    {
        if (onMovePressed != null)
        {
            onMovePressed(moveDir);
        }
    }

    // Look Input (e.g., camera control)
    public event Action<Vector2> onLookPressed;
    public void LookPressed(Vector2 lookInput)
    {
        if (onLookPressed != null)
        {
            onLookPressed(lookInput);
        }
    }

    // Quest Log Toggle
    public event Action onQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        if (onQuestLogTogglePressed != null)
        {
            onQuestLogTogglePressed();
        }
    }

    // Inventory Toggle
    public event Action onInventoryTogglePressed;
    public void InventoryTogglePressed()
    {
        if (onInventoryTogglePressed != null)
        {
            onInventoryTogglePressed();
        }
    }

    // Inventory Navigation
    public event Action<Vector2> onNavigateInventory;
    public void NavigateInventory(Vector2 direction)
    {
        if (onNavigateInventory != null)
        {
            onNavigateInventory(direction);
        }
    }

    // Inventory Item Selection
    public event Action onSelectInventoryItem;
    public void SelectInventoryItem()
    {
        if (onSelectInventoryItem != null)
        {
            onSelectInventoryItem();
        }
    }

    // Jump Input
    public event Action onJumpPressed;
    public void JumpPressed()
    {
        if (onJumpPressed != null)
        {
            onJumpPressed();
        }
    }

    // Interact Input
    public event Action onInteractPressed;
    public void InteractPressed()
    {
        if (onInteractPressed != null)
        {
            onInteractPressed();
        }
    }

    public event Action onSprintPressed;
    public void SprintPressed()
    {
        if (onSprintPressed != null)
        {
            onSprintPressed();
        }
    }

    public event Action onSprintReleased;
    public void SprintReleased()
    {
        if (onSprintReleased != null)
        {
            onSprintReleased();
        }
    }

    public event Action onAttackPressed;
    public void AttackPressed()
    {
        if (onAttackPressed != null)
        {
            onAttackPressed();
        }
    }

    public event Action onRunPressed;
    public void RunPressed()
    {
        if (onRunPressed != null)
        {
            onRunPressed();
        }
    }

    public event Action onRunReleased;
    public void RunReleased()
    {
        if (onRunReleased != null)
        {
            onRunReleased();
        }
    }

    public event Action onWalkTogglePressed;
    public void WalkTogglePressed()
    {
        if (onWalkTogglePressed != null)
        {
            onWalkTogglePressed();
        }
    }

    public event Action onPreviousPressed;
    public void PreviousPressed()
    {
        if (onPreviousPressed != null)
        {
            onPreviousPressed();
        }
    }
    
    
}
