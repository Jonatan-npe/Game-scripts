using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable] public class AttackVariant
{
    public string animationName;
    public int colliderIndex;       // 0 = attack1Collider, 1 = attack2Collider, ...
    public float damageMultiplier = 1f;
}
[Serializable] public class AttackType
{
    public List<AttackVariant> attacks = new List<AttackVariant>();
    public string attackTypeName;
    public float damageMultiplier = 1f;

}
/// <summary>
/// Clase que maneja el sistema de ataques, y sus respectivos combos.
/// </summary>
public class ComboSystem : CombatantBase
{
    // Array de nombres de animaciones para cada combo

    [SerializeField] private List<AttackType> attackTypes = new List<AttackType>();
    public List<AttackType> AttackTypes
    {
        get { return attackTypes; }
    }
    private int currentAttackIndex = 0;
    private int currentAttackTypeIndex = 0;
    [SerializeField, Range(0, 300)] private float framesToEndCombo;
    private Coroutine endComboCoroutine;

    // Scripts internos
    private ICombatant combatant;
    private Animator animator;
    
    public int GetCurrentTypeIndex()
    {
        return currentAttackTypeIndex;
    }
    public int GetCurrentAttackIndex()
    {
        return currentAttackIndex;
    }
    void Awake()
    {
        animator = GetComponent<Animator>();
        combatant = GetComponent<ICombatant>();
        if (combatant != null)
        {
            combatant.OnAttack += HandleComboChange;
        }
        if (endComboCoroutine != null)
        {
            StopCoroutine(endComboCoroutine);
        }
        endComboCoroutine = StartCoroutine(EndCombo());
    }

    private void HandleComboChange()
    {
        // obtener la animación del combo actual
        var currentAttackType = attackTypes[currentAttackTypeIndex];
        if (currentAttackType.attacks.Count == 0) return;

        var attack = currentAttackType.attacks[currentAttackIndex];
        animator.SetTrigger(attack.animationName);

        currentAttackIndex = (currentAttackIndex + 1) % currentAttackType.attacks.Count;
    
        if (endComboCoroutine != null)
            StopCoroutine(endComboCoroutine);
        endComboCoroutine = StartCoroutine(EndCombo());

        Debug.Log($"Combo cambió a índice: {currentAttackIndex}, Animación: {attack.animationName}");
    }

    /// <summary>
    /// Retorna el índice actual del combo (0, 1, 2, 3...)
    /// </summary>

    private IEnumerator EndCombo()
    {
        Debug.Log("EndCombo started");
        yield return FrameWaiter(framesToEndCombo);
        Debug.Log("EndCombo finished");
        currentAttackIndex = 0; // Reinicia al primer combo
    }
    public IEnumerator FrameWaiter(float duration)
    {
        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }
}
