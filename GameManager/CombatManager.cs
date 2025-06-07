using Mono.Cecil;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public void RegisterHit(GameObject attacker, GameObject target, float damage)
    {
        Debug.Log($"{attacker.name} golpea {target.name}");
        if (target.layer == LayerMask.NameToLayer("EnemyToPlayer"))
        {
            if (target.TryGetComponent<Enemy>(out var enemy))
            {
                // Aquí puedes aplicar daño al enemigo
                enemy.GetDamage(damage);
            }
            else
            {
                Debug.LogWarning("El objeto golpeado no tiene un componente Enemy.");
            }
        }
        else
        {
            CombatPlayer combatPlayer = target.GetComponent<CombatPlayer>();
        }
       
    }
}
        