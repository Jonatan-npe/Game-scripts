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
    readonly Dictionary<AttackOnCombo, string> witchAttack = new()
    {
        {AttackOnCombo.ComboAttack1 , "Attack1"},
        {AttackOnCombo.ComboAttack2 , "Attack2" }
    };

    //Variables editables
    [SerializeField, Range(0, 100)] private float framesToAttackAgain;
    [SerializeField, Range(0, 300)] private float framesToEndCombo;

    //colliders
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private PolygonCollider2D attack1Collider;
    [SerializeField] private PolygonCollider2D attack2Collider;


    //Componentes internos del pj
    private Rigidbody2D rigidbody2DPlayer;
    private Animator animator;
    private PlayerInput playerInput;
    private MainPlayer mainPlayerScript;

    //Variables no editables
    private float framesSinceLastAttack = 0;
    private bool canAttack = true;
    private bool isAttackActive = false; // Se activa/desactiva desde la animación
    private bool hasHitInThisWindow = false; // Flag para evitar múltiples hits
    private HashSet<GameObject> enemiesHitThisWindow = new(); // Para registrar enemigos golpeados
    private List<Collider2D> hitResults = new(); // Para almacenar los colliders detectados
    private int witchAttackIndex = 0; // Para llevar el control del ataque actual

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2DPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        mainPlayerScript = GetComponent<MainPlayer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttackActive && !hasHitInThisWindow)
        {
            CheckHit();
        }
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

    private void CheckHit()
    {
        int hitCount = 0;
        switch (witchAttackIndex)
        {
            case 0:
                hitCount = Physics2D.OverlapCollider(attack1Collider, hitResults);
                break;
            case 1:
                hitCount = Physics2D.OverlapCollider(attack2Collider, hitResults);
                break;
            default:
                break;
        }
        for (int i = 0; i < hitCount; i++)
        {
            GameObject enemy = hitResults[i].gameObject;
            if (!enemiesHitThisWindow.Contains(enemy))
            {
                enemiesHitThisWindow.Add(enemy);
                GameManager.Instance.RegisterHit(this.gameObject, enemy, mainPlayerScript.CurrentDamage);
            }
        }
        hasHitInThisWindow = true;

    }

    public void GetDamage(float damage)
    {
        mainPlayerScript.GetDamage(damage);
    }

    //funciones llamads por el animator
    public void StartAttack1Window()
    {
        isAttackActive = true;
        hasHitInThisWindow = false;
        enemiesHitThisWindow.Clear();
        witchAttackIndex = 0;
    }
    public void StartAttack2Window()
    {
        isAttackActive = true;
        hasHitInThisWindow = false;
        enemiesHitThisWindow.Clear();
        witchAttackIndex = 1;
    }

    public void EndAttackWindow()
    {
        isAttackActive = false;
    }

}
