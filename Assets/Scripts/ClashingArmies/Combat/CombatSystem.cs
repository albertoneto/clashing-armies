using System;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatSystem : MonoBehaviour
    {
        public event Action<Vector3> OnVictory;
        
        private CombatHierarchy _combatHierarchy;
        private UnitController _controller;
        private CombatDetector _combatDetector;
        private CombatResolver _combatResolver;
        
        private float _combatCooldown = 1f;
        private float _lastCombatTime = -999f;
        
        public void Initialize(UnitController controller, CombatHierarchy combatHierarchy, UnitsManager unitsManager)
        {
            if (combatHierarchy == null)
            {
                Debug.LogError("Combat Hierarchy not assigned! Please assign it in the inspector.");
                return;
            }
            
            _controller = controller;
            _combatHierarchy = combatHierarchy;
            _combatDetector = new CombatDetector(transform, _controller.Unit.data.layer, _controller.Unit.data.detectionRadius, unitsManager);
            _combatResolver = new CombatResolver(_combatHierarchy);
        }
        
        private void Update()
        {
            if (_combatDetector == null) return;
            
            Unit enemy = _combatDetector.FindNearestEnemy();
            if (enemy == null) return;
            
            TryEngageCombat(enemy);
        }
        
        private void TryEngageCombat(Unit enemy)
        {
            if (Time.time - _lastCombatTime < _combatCooldown) return;
            
            var enemyController = enemy.controller;
            var enemyCombatSystem = enemyController.combatSystem;
            if (!enemyCombatSystem) return;
            
            _lastCombatTime = Time.time;
            CombatResult result = _combatResolver.ResolveCombat(_controller.Unit, enemyController.Unit);
            ApplyCombatResult(result);
        }
        
        private void ApplyCombatResult(CombatResult result)
        {
            result.Winner.health.TakeDamage(result.DamageDealt);
            result.Loser.health.TakeDamage(result.Loser.health.MaxHealth);
            
             OnVictory?.Invoke(result.Winner.UnitObject.transform.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _controller.Unit.data.detectionRadius);
        }
    }
#endif
}