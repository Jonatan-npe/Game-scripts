using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float scaleMove;

    private PlayerInput playerInput;
    private Vector2 originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["Look"].ReadValue<Vector2>().y != 0)
        {
            transform.localPosition = new Vector2(originalPosition.x, originalPosition.y + (playerInput.actions["Look"].ReadValue<Vector2>().y*scaleMove));
        }
    }
}
