using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegen = 10f;
    [SerializeField] private float damageBase = 10f;

    private float currentHealth;
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
    private float currentDamage;
    public float CurrentDamage
    {
        get { return currentDamage; }
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
        Debug.Log("Player took damage: " + damage + ", Current Health: " + CurrentHealth);
    }
}
