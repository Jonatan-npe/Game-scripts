using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy : CombatantBase
{
    [SerializeField] private EnemyData enemyData;

    //Componentes internos del enemy
    private Rigidbody2D enemyRigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned in the inspector for " + gameObject.name);
            return;
        }
        currentHealth = enemyData.health;
    }

    /// <summary>
    /// Sobrescribe GetDamage para aplicar knockback específico de enemigos.
    /// </summary>
    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);
    }

    /// <summary>
    /// Versión con posición que aplica knockback al enemigo.
    /// </summary>
    public override void GetDamage(float damage, Vector2 position)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            currentHealth = 0;

        Debug.Log($"{gameObject.name} recibió {damage} de daño desde {position}. Salud: {currentHealth}");

        // Aplicar knockback
        if (enemyRigidbody2D != null)
        {
            Vector2 knockbackDirection = ((Vector2)transform.position - position).normalized;
            enemyRigidbody2D.AddForce(knockbackDirection * enemyData.knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Sobrescribe OnDeath para lógica de muerte específica del enemigo.
    /// </summary>
    protected override void OnDeath()
    {
        Debug.Log(gameObject.name + " has died.");
        //Logica de la muerte del enemigo	
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "EnemyCollider")
        {
            GameManager.Instance.RegisterHit(gameObject, collision.gameObject, enemyData.damage, transform.position);
        }
    }
    
}
