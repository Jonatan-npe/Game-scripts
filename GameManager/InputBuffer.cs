using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer : MonoBehaviour
{
    Queue<InputAction> inputQueueMove = new();
    Queue<InputAction> inputQueueAttack = new();

    public void AddActionMove(InputAction input)
    {
        inputQueueMove.Enqueue(input);
    }
    public InputAction GetLastInputMovement()
    {
        if (inputQueueMove.Count == 0) return null;
        else
        {
            InputAction input = inputQueueMove.Peek();
            inputQueueMove.Dequeue();
            return input;
        }
    }
    public void AddActionAttack(InputAction input)
    {
        inputQueueAttack.Enqueue(input);
    }
    public InputAction GetLastInputAttack()
    {
        if (inputQueueAttack.Count == 0) return null;
        else
        {
            InputAction input = inputQueueAttack.Peek();
            inputQueueAttack.Dequeue();
            return input;
        }
    }
}
