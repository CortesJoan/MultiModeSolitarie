using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSelectionManager : MonoBehaviour
{ 
    private Vector3 mousePosition;
    public event Action<Vector2> onCardMovementEvent;
    public event Action onCardSelected;
    public event Action onCardUnSelected;
    public event Action onNextCardEvent;
    public event Action onPreviousCardEvent;

 
 

    public void OnPositionChanged(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        onCardMovementEvent?.Invoke(mousePosition);
    }

    public void OnCardSelected(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        onCardSelected?.Invoke();
    }

    public void OnCardUnSelected(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        onCardUnSelected?.Invoke();
    }
 
    public void OnNextCard(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        onNextCardEvent?.Invoke();
    }

    public void OnPreviousCard(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        onPreviousCardEvent?.Invoke();
    }
}