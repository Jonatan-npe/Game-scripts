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
    [SerializeField, Range(0, 200)] private float framesToGetDamage;

    //colliders
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private PolygonCollider2D attack1Collider;
    [SerializeField] private PolygonCollider2D attack2Collider;
    [SerializeField] private CapsuleCollider2D damageCollider; // Collider para recibir daño

    [SerializeField, Range(0, 1000)] private float forceBounce = 100f; // Fuerza de rebote al recibir daño

    //Componentes internos del pj
    private Rigidbody2D rigidbody2DPlayer;
    private Animator animator;
    private PlayerInput playerInput;
    private MainPlayer mainPlayerScript;

    //Variables no editables
    private bool canAttack = true;
    private bool isAttackActive = false; // Se activa/desactiva desde la animación
    private bool hasHitInThisWindow = false; // Flag para evitar múltiples hits
    private HashSet<GameObject> enemiesHitThisWindow = new(); // Para registrar enemigos golpeados
    private List<Collider2D> hitResults = new(); // Para almacenar los colliders detectados
    private int witchAttackIndex = 0; // Para llevar el control del ataque actual

    private Coroutine endComboCoroutine;


    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2DPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        mainPlayerScript = GetComponent<MainPlayer>();
        if (endComboCoroutine != null)
        {
            StopCoroutine(endComboCoroutine);
        }
        endComboCoroutine = StartCoroutine(EndCombo());
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
        InputAction lastInput = GameManager.Instance.GetLastInputAttack();
        if (lastInput != null)
        {
            ActionAttackBuffer(lastInput);
        }
    }
private void ActionAttackBuffer(InputAction lastInput)
{
    if (!canAttack) return;

    switch (lastInput.name)
    {
        case "Attack":
            switch (attackType)
            {
                case AttackOnCombo.ComboAttack1:
                    animator.SetTrigger(witchAttack[AttackOnCombo.ComboAttack1]);
                    attackType = AttackOnCombo.ComboAttack2;
                    StartCoroutine(WaitForAttack());
                    if (endComboCoroutine != null)
                        StopCoroutine(endComboCoroutine);
                    endComboCoroutine = StartCoroutine(EndCombo());
                    break;
                case AttackOnCombo.ComboAttack2:
                    animator.SetTrigger(witchAttack[AttackOnCombo.ComboAttack2]);
                    StartCoroutine(WaitForAttack());
                    attackType = AttackOnCombo.ComboAttack1;
                    if (endComboCoroutine != null)
                        StopCoroutine(endComboCoroutine);
                    break;
            }
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
                GameManager.Instance.RegisterHit(this.gameObject, enemy, mainPlayerScript.CurrentDamage, transform.position);
            }
        }
        hasHitInThisWindow = true;

    }

    public void GetDamage(float damage)
    {
        mainPlayerScript.GetDamage(damage);
    }
    public void GetDamage(float damage, Vector2 position)
    {
        rigidbody2DPlayer.AddForce(((Vector2)transform.position - position).normalized * forceBounce, ForceMode2D.Impulse);
        mainPlayerScript.GetDamage(damage);
        StartCoroutine(mainPlayerScript.InvulnerabilityFrames(framesToGetDamage));
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

    private IEnumerator WaitForAttack()
    {
        canAttack = false;
        Debug.Log("WaitForAttack started");
        yield return mainPlayerScript.FrameWaiter(framesToAttackAgain);
        Debug.Log("WaitForAttack finished");
        canAttack = true;
    }
    private IEnumerator EndCombo()
    {
        Debug.Log("EndCombo started");
        yield return mainPlayerScript.FrameWaiter(framesToEndCombo);
        Debug.Log("EndCombo finished");
        attackType = AttackOnCombo.ComboAttack1;
    }

}
