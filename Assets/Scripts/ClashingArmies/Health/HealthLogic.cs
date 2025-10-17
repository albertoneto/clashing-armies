using System;
using UnityEngine;

namespace ClashingArmies.Health
{
    public class HealthLogic : IHealthSystem
    {
        private float currentHealth;
        private float maxHealth;
    
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => currentHealth <= 0;
    
        public event Action<float, float> OnHealthChanged;
        public event Action<float> OnDamageTaken;
        public event Action<float> OnHealed;
        public event Action OnDeath;
    
        public HealthLogic(float initialMaxHealth)
        {
            maxHealth = initialMaxHealth;
            currentHealth = maxHealth;
        }
    
        public void TakeDamage(float amount)
        {
            if (IsDead) return;
        
            amount = Mathf.Max(0, amount);
            currentHealth = Mathf.Max(0, currentHealth - amount);
        
            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }
    
        public void Heal(float amount)
        {
            if (IsDead) return;
        
            amount = Mathf.Max(0, amount);
            float oldHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        
            float actualHealed = currentHealth - oldHealth;
            if (actualHealed > 0)
            {
                OnHealed?.Invoke(actualHealed);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
    
        public void SetMaxHealth(float newMax)
        {
            newMax = Mathf.Max(1, newMax);
            float healthPercentage = currentHealth / maxHealth;
        
            maxHealth = newMax;
            currentHealth = maxHealth * healthPercentage;
        
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? currentHealth / maxHealth : 0;
        }
    }
}