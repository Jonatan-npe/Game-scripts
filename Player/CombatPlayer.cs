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
    Dictionary<AttackOnCombo,string> witchAttack = new Dictionary<AttackOnCombo, string>()
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

    private float lastAttack;

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
        if (attackType == AttackOnCombo.ComboAttack2)
        {
            float lastestAttack = lastAttack + (framesToEndCombo * Time.deltaTime);
            if (lastestAttack <= Time.time)
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
        switch (lastInput.name)
        {
            case "Attack":
                switch (attackType)
                {
                    case AttackOnCombo.ComboAttack1:

                        animator.SetTrigger(witchAttack[AttackOnCombo.ComboAttack1]);
                        lastAttack = Time.time;
                        attackType = AttackOnCombo.ComboAttack2;

                        break;
                    case AttackOnCombo.ComboAttack2:

                        animator.SetTrigger(witchAttack[AttackOnCombo.ComboAttack2]);
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
