using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 10f;
    [SerializeField] private float damageBase = 10f;

    [SerializeField, ReadOnly]private float currentHealth;
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

    // Componentes internos del jugador
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
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
}
