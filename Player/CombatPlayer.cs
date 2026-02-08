using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Enums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(ComboSystem))]
public class CombatPlayer : CombatantBase
{
    //Variables de combate (salud y daño)
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 10f;
    [SerializeField] private float damageBase = 10f;
    [SerializeField, ReadOnly] private float currentDamage;
    public float CurrentDamage
    {
        get { return currentDamage; }
    }

    //Variables editables de ataque
    [SerializeField, Range(0, 60)] private float framesToAttackAgain = 30f;
    [SerializeField, Range(0, 60)] private float framesToEndCombo;
    [SerializeField, Range(0, 60)] private float framesToGetDamage;
    [SerializeField, Range(0, 60)] private float earlyPressIgnoreWindow = 10f; // Frames iniciales: se ignora
    [SerializeField, Range(0, 60)] private float latePressCaptureWindow = 20f; // Últimos frames: se guarda

    //colliders
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private List<PolygonCollider2D> attackColliders = new();
    [SerializeField] private CapsuleCollider2D damageCollider; // Collider para recibir daño

    [SerializeField, Range(0, 1000)] private float forceBounce = 100f; // Fuerza de rebote al recibir daño

    //Color para indicar invulnerabilidad
    [SerializeField] private Color invulnerabilityColor; // Color con alpha reducido

    //Componentes internos del pj
    private Rigidbody2D rigidbody2DPlayer;
    private Animator animator;
    private PlayerInput playerInput;
    private MainPlayer mainPlayerScript;
    private ComboSystem comboSystem;

    //Variables no editables
    private bool canAttack = true;
    private bool isAttackActive = false; // Se activa/desactiva desde la animación
    private bool hasHitInThisWindow = false; // Flag para evitar múltiples hits
    private bool hasPendingAttack = false; // Indica si hay un ataque en cola
    private float framesSinceLastAttack = 0f; // Tiempo desde el último ataque
    private InputAction pendingAttackAction; // La acción guardada
    private HashSet<GameObject> enemiesHitThisWindow = new(); // Para registrar enemigos golpeados
    private List<Collider2D> hitResults = new(); // Para almacenar los colliders detectados
    InputAction lastInput;

    // Eventos
    // Start is called before the first frame update
    void Awake()
    {
        comboSystem = GetComponent<ComboSystem>();
        rigidbody2DPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        mainPlayerScript = GetComponent<MainPlayer>();
        animator.SetBool("Dead", false);
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentDamage = damageBase;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAttackActive && !hasHitInThisWindow)
        {
            CheckHit();
        }
        if (!canAttack)
        {
            framesSinceLastAttack += 1f;
        }
    }
    private void Update()
    {
        InputAction lastInput = GameManager.Instance.GetLastInputAttack();
        if (lastInput != null)
        {
            ActionAttackBuffer(lastInput);
        }
        if (hasPendingAttack && canAttack)
        {
            ExecutePendingAttack();
        }
    }
    private void ExecutePendingAttack()
    {
        hasPendingAttack = false;
        RaiseOnAttack();
        StartCoroutine(WaitForAttack());
    }
    private void ActionAttackBuffer(InputAction lastInput)
    {
        float remainingFrames = framesToAttackAgain - framesSinceLastAttack;
        if (!canAttack && framesSinceLastAttack < earlyPressIgnoreWindow) return;
        switch (lastInput.name)
        {
            case "Attack":
                if (remainingFrames <= latePressCaptureWindow)
                {
                    hasPendingAttack = true;
                    pendingAttackAction = lastInput;
                    return;
                }
                RaiseOnAttack();
                StartCoroutine(WaitForAttack());
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
        var currentAttackTypeIndex = comboSystem.GetCurrentTypeIndex();
        var currentAttackIndex = comboSystem.GetCurrentAttackIndex();
        hitCount = Physics2D.OverlapCollider(attackColliders[comboSystem.AttackTypes[currentAttackTypeIndex]
                                                                        .attacks[currentAttackIndex].colliderIndex], hitResults);

        for (int i = 0; i < hitCount; i++)
        {
            GameObject enemy = hitResults[i].gameObject;
            if (!enemiesHitThisWindow.Contains(enemy))
            {
                enemiesHitThisWindow.Add(enemy);
                GameManager.Instance.RegisterHit(this.gameObject, enemy, CurrentDamage, transform.position);
            }
        }
        hasHitInThisWindow = true;

    }

    /// <summary>
    /// Sobrescribe GetDamage para centralizar la lógica de daño del jugador.
    /// </summary>
    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
        GetComponent<Animator>().SetTrigger("Hit");
    }

    /// <summary>
    /// Versión con knockback para el jugador.
    /// </summary>
    public override void GetDamage(float damage, Vector2 position)
    {
        rigidbody2DPlayer.AddForce(((Vector2)transform.position - position).normalized * forceBounce, ForceMode2D.Impulse);
        base.GetDamage(damage);
        GetComponent<Animator>().SetTrigger("Hit");
        StartCoroutine(mainPlayerScript.InvulnerabilityFrames(framesToGetDamage, damageCollider, invulnerabilityColor));
    }

    /// <summary>
    /// Sobrescribe OnDeath para lógica específica del jugador.
    /// </summary>
    protected override void OnDeath()
    {
        Debug.Log("Player has died.");
        GetComponent<Animator>().SetBool("Dead", true);
        //Logica de la muerte del juegador
    }

    //funciones llamads por el animator
    public void StartAttack1Window()
    {
        isAttackActive = true;
        hasHitInThisWindow = false;
        enemiesHitThisWindow.Clear();
    }
    public void StartAttack2Window()
    {
        isAttackActive = true;
        hasHitInThisWindow = false;
        enemiesHitThisWindow.Clear();
    }

    public void EndAttackWindow()
    {
        isAttackActive = false;
    }

    /// <summary>
    /// Espera un número de frames especificado.
    /// </summary>
    public IEnumerator FrameWaiter(float duration)
    {
        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Corrutina para frames de invulnerabilidad tras recibir daño.
    /// </summary>


    private IEnumerator WaitForAttack()
    {
        canAttack = false;
        framesSinceLastAttack = 0f;
        Debug.Log("WaitForAttack started");
        yield return FrameWaiter(framesToAttackAgain);
        Debug.Log("WaitForAttack finished");
        canAttack = true;
    }
}
