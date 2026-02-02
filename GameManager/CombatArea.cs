using UnityEngine;

/// <summary>
/// Script de ejemplo que muestra cómo aplicar daño a todos los CombatantBase cercanos.
/// Útil para explosiones, AoE (Area of Effect), etc.
/// </summary>
public class CombatArea : MonoBehaviour
{
    [SerializeField] private float damageRadius = 5f;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private LayerMask combatantLayer; // Asigna las capas de Player y Enemies

    /// <summary>
    /// Aplica daño a todos los combatientes dentro del radio especificado.
    /// </summary>
    public void DealAreaDamage(Vector2 center)
    {
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(center, damageRadius, combatantLayer);
        
        Debug.Log($"Encontrados {collidersInRadius.Length} combatientes en el área de daño.");

        foreach (Collider2D collider in collidersInRadius)
        {
            CombatantBase combatant = collider.GetComponent<CombatantBase>();
            
            if (combatant != null)
            {
                // Calcular distancia desde el centro del área
                Vector2 directionToTarget = (Vector2)combatant.transform.position - center;
                
                // Aplicar daño (opcional: reducir según distancia)
                combatant.GetDamage(damageAmount, center);
                
                Debug.Log($"Daño aplicado a {combatant.gameObject.name}: -{damageAmount}");
            }
        }
    }

    /// <summary>
    /// Alternativa: aplica daño solo al combatiente más cercano.
    /// </summary>
    public void DealDamageToClosest(Vector2 center)
    {
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(center, damageRadius, combatantLayer);
        
        if (collidersInRadius.Length == 0)
        {
            Debug.LogWarning("No hay combatientes en el área de daño.");
            return;
        }

        CombatantBase closestCombatant = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in collidersInRadius)
        {
            CombatantBase combatant = collider.GetComponent<CombatantBase>();
            
            if (combatant != null)
            {
                float distance = Vector2.Distance(combatant.transform.position, center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCombatant = combatant;
                }
            }
        }

        if (closestCombatant != null)
        {
            closestCombatant.GetDamage(damageAmount, center);
            Debug.Log($"Daño aplicado al combatiente más cercano {closestCombatant.gameObject.name}: -{damageAmount}");
        }
    }

    /// <summary>
    /// Visualiza el área de daño en el editor (para debugging).
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
