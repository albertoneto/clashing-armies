using System;
using UnityEngine;

namespace ClashingArmies.Health
{
    public class HealthSystem
    {
    
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDead => _currentHealth <= 0;
    
        public event Action<float, float> OnHealthChanged;
        public event Action<float> OnDamageTaken;
        public event Action OnDeath;
        
        private float _currentHealth;
        private float _maxHealth;
    
        public HealthSystem(float initialMaxHealth)
        {
            _maxHealth = initialMaxHealth;
            _currentHealth = _maxHealth;
        }
    
        public void TakeDamage(float amount)
        {
            if (IsDead) return;
        
            amount = Mathf.Max(0, amount);
            _currentHealth = Mathf.Max(0, _currentHealth - amount);
        
            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        
            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }
    
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }
}