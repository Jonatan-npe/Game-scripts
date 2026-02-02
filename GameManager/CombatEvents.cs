using UnityEngine;

public class CombatEvents : MonoBehaviour
{
    private CombatantBase combatant;
    
    void Awake()
    {
        // Obtén el componente CombatantBase (funciona con CombatPlayer, Enemy, o cualquier subclase)
        combatant = GetComponent<CombatantBase>();
        
        if (combatant != null)
        {
            Debug.Log($"CombatantBase encontrado en {gameObject.name}: {combatant.GetType().Name}");
            Debug.Log($"Salud actual: {combatant.CurrentHealth}");
        }
        else
        {
            Debug.LogWarning($"No se encontró CombatantBase en {gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
