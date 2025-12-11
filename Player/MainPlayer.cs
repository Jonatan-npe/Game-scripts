using UnityEngine;
using System.Collections;

public class MainPlayer : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 10f;
    [SerializeField] private float damageBase = 10f;
    [SerializeField, ReadOnly] private float currentHealth;
    //Colliders
    [SerializeField] private CapsuleCollider2D damageCollider; // Collider para recibir daño

    //Color para indicar invulnerabilidad
    [SerializeField] private Color invulnerabilityColor; // Color con alpha reducido
    public float CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }
    [SerializeField, ReadOnly] private float currentDamage;
    public float CurrentDamage
    {
        get { return currentDamage; }
    }


    // Eventos y declaraciones necesarias para los eventos
    public event System.Action<float> OnDirectionChanged;
    private float lastDirection = 1f;


    // Componentes internos del jugador
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MovementPlayer movementPlayerScript;
    private Transform playerTransform;
    public Transform PlayerTransform
    {
        get { return playerTransform; }
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementPlayerScript = GetComponent<MovementPlayer>();
        playerTransform = GetComponent<Transform>();
        animator.SetBool("Dead", false);
    }
    private void Start()
    {
        CurrentHealth = maxHealth;
        currentDamage = damageBase;
    }
    private void Die()
    {
        Debug.Log("Player has died.");
        //Logica de la muerte del juegador
    }
    public void GetDamage(float damage)
    {
        CurrentHealth -= damage;
        animator.SetTrigger("Hit");
        Debug.Log("Player took damage: " + damage + ", Current Health: " + CurrentHealth);
    }
    //Coroutines
    public IEnumerator InvulnerabilityFrames(float duration)
    {
        damageCollider.enabled = false; // Desactiva el collider para evitar daño
        spriteRenderer.color = invulnerabilityColor; // Cambia el color del sprite para indicar invulnerabilidad
        yield return FrameWaiter(duration); // Espera el número de frames especificado
        spriteRenderer.color = Color.white; // Restaura el color original del sprite
        damageCollider.enabled = true; // Reactiva el collider después de los frames de invulnerabilidad
    }

    public IEnumerator FrameWaiter(float duration)
    {
        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForFixedUpdate(); // Espera un frame
        }
    }

    public void DisableMovement()
    {
        movementPlayerScript.CanMove = false;
    }
    public void EnableMovement()
    {
        movementPlayerScript.CanMove = true;
    }
    // funcion es de los eventos

    public void NotifyDirectionChange(float newDirection)
    {
        if (newDirection != lastDirection)
        {
            lastDirection = newDirection;
            OnDirectionChanged?.Invoke(newDirection);
        }
    }
}
