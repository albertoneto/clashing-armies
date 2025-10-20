using UnityEngine;

namespace ClashingArmies.Combat
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }
        
        void TakeDamage(float amount);
        GameObject GameObject { get; }
    }
}