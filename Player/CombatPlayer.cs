using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatPlayer : MonoBehaviour
{
    //Tipos de ataques en una lista para su facil acceso
    private enum AttackOnCombo
    {
        ComboAttack1,
        ComboAttack2
    }
    private AttackOnCombo attackType = new();
    readonly Dictionary<AttackOnCombo,string> witchAttack = new()
    {
        {AttackOnCombo.ComboAttack1 , "Attack1"},
        {AttackOnCombo.ComboAttack2 , "Attack2" }
    };

    //Variables editables
    [SerializeField, Range(0, 100)] private float framesToAttackAgain;
    [SerializeField, Range(0 , 300)] private float framesToEndCombo;

    //Componentes internos del pj
    private Rigidbody2D rigidbody2DPlayer;
    private Animator animator;
    private PlayerInput playerInput;

    //Variables no editables
    private float framesSinceLastAttack = 0;
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2DPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    private void Update()
    {
        framesSinceLastAttack++;

        // Cooldown: solo permite atacar si han pasado los frames necesarios
        if (framesSinceLastAttack < framesToAttackAgain)
            canAttack = false;
        else
            canAttack = true;

        if (attackType == AttackOnCombo.ComboAttack2)
        {
            if (framesSinceLastAttack >= framesToEndCombo)
            {
                attackType = AttackOnCombo.ComboAttack1;
            }
        }

        InputAction lastInput = GameManager.Instance.GetLastInputAttack();
        if (lastInput != null)
        {
            ActionAttackBuffer(lastInput);
        }
    }

    private void ActionAttackBuffer(InputAction lastInput)
    {
        if (!canAttack) return; // <-- Solo procesa si el cooldown terminó

        switch (lastInput.name)
        {
            case "Attack":
                switch (attackType)
                {
                    case AttackOnCombo.ComboAttack1:
                        animator.SetTrigger(witchAttack[AttackOnCombo.ComboAttack1]);
                        framesSinceLastAttack = 0;
                        attackType = AttackOnCombo.ComboAttack2;
                        break;
                    case AttackOnCombo.ComboAttack2:
                        animator.SetTrigger(witchAttack[AttackOnCombo.ComboAttack2]);
                        framesSinceLastAttack = 0;
                        attackType = AttackOnCombo.ComboAttack1;
                        break;
                }
                Debug.Log("Ataca");
                break;
        }
    }
    public void RequestToAtack(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            GameManager.Instance.AddBufferAttack(callbackContext.action);

        }
    }
    private IEnumerator CoolDownToAttack()
    {
        yield return new WaitForSecondsRealtime(5);
    }

}
