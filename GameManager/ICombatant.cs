using System;

public interface ICombatant
{
    // Eventos
    event Action OnAttack;

    float CurrentHealth { get; }
    void GetDamage(float damage);
}
