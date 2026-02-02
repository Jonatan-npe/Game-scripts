using UnityEngine;
using System;

/// <summary>
/// Clase base abstracta para todos los combatientes (Jugador, Enemigos, etc).
/// Centraliza la lógica común de daño, salud y muerte.
/// </summary>
public abstract class CombatantBase : MonoBehaviour, ICombatant
{
    // Eventos
    public event Action OnAttack;
    [SerializeField, ReadOnly] protected float currentHealth;
    
    public float CurrentHealth
    {
        get { return currentHealth; }
        protected set { currentHealth = value; }
    }
    /// <summary>
    /// Dispara el evento OnAttack. Llamar desde subclases cuando se ataque.
    /// </summary>
    protected void RaiseOnAttack()
    {
        OnAttack?.Invoke();
    }
    /// <summary>
    /// Aplica daño al combatiente. Implementación base que resta salud.
    /// Las subclases pueden extender este método para añadir lógica específica.
    /// </summary>
    public virtual void GetDamage(float damage)
    {
        if (damage < 0) damage = 0;
        
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Salud actual: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath();
        }
    }

    /// <summary>
    /// Versión de GetDamage que incluye posición (útil para knockback, efectos, etc).
    /// </summary>
    public virtual void GetDamage(float damage, Vector2 position)
    {
        GetDamage(damage);
    }

    /// <summary>
    /// Método llamado cuando la salud llega a 0. Las subclases pueden sobrescribir para lógica específica.
    /// </summary>
    protected virtual void OnDeath()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        // Aquí pueden ir efectos, sonidos, animaciones, etc.
    }

    /// <summary>
    /// Restaura salud. Útil para pociones, regeneración, etc.
    /// </summary>
    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        Debug.Log($"{gameObject.name} fue curado por {amount}. Salud actual: {currentHealth}");
    }
}
