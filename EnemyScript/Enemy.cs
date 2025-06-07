using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField, ReadOnly] private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    // Start is called before the first frame update
    void Start()
    {
        CustomCollider2D detectionCollider = GetComponentInChildren<CustomCollider2D>();
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned in the inspector for " + gameObject.name);
            return;
        }
        currentHealth = enemyData.health;
    }
    public void GetDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
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
        
    }
    
}
