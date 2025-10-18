using System;
using UnityEngine;

namespace ClashingArmies.Health
{
    public class HealthSystem
    {
        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; }

        private bool IsDead => CurrentHealth <= 0;

        public HealthSystem(float initialMaxHealth)
        {
            MaxHealth = initialMaxHealth;
            CurrentHealth = MaxHealth;
        }
    
        public void TakeDamage(float amount)
        {
            if (IsDead) return;
        
            amount = Mathf.Max(0, amount);
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        
            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }
    
        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}