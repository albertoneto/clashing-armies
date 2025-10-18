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
        public event Action<float> OnHealed;
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
    
        public void Heal(float amount)
        {
            if (IsDead) return;
        
            amount = Mathf.Max(0, amount);
            float oldHealth = _currentHealth;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        
            float actualHealed = _currentHealth - oldHealth;
            if (actualHealed > 0)
            {
                OnHealed?.Invoke(actualHealed);
                OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }
    
        public void SetMaxHealth(float newMax)
        {
            newMax = Mathf.Max(1, newMax);
            float healthPercentage = _currentHealth / _maxHealth;
        
            _maxHealth = newMax;
            _currentHealth = _maxHealth * healthPercentage;
        
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    
        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    
        public float GetHealthPercentage()
        {
            return _maxHealth > 0 ? _currentHealth / _maxHealth : 0;
        }
    }
}