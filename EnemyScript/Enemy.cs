using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private float currentHealth;

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

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay2D(Collider2D collision)
    {
        
    }
    
}
