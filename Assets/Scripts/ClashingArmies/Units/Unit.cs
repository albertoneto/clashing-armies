using ClashingArmies.Combat;
using ClashingArmies.Health;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class Unit : ICombatant
    {
        public string id;
        public GameObject UnitObject;
        public UnitData data;
        public UnitController controller;
        public UnitView view;
        public HealthSystem health;

        public UnitType UnitType => data.unitType;
        public UnitController Controller => controller;
        public int CombatLayer => Mathf.RoundToInt(Mathf.Log(data.layer.value, 2));
        public float DetectionRadius => data.detectionRadius;
        
        public float CurrentHealth => health.CurrentHealth;
        public float MaxHealth => health.MaxHealth;
        public bool IsDead => health.CurrentHealth <= 0;
        public GameObject GameObject => UnitObject;

        public void TakeDamage(float amount)
        {
            health.TakeDamage(amount);
        }

        public bool CanEngageCombat()
        {
            return !IsDead && controller != null;
        }

        public void OnCombatVictory()
        {
            //view?.PlayVictoryEffect();
        }

        public void OnCombatDefeat()
        {
            //death is already handled by HealthSystem.OnDeath event
        }

        public void Dispose()
        {
            health.OnDeath -= controller.HandleDeath;
        }
    }
}