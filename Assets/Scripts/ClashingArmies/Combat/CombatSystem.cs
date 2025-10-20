using System;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatSystem : MonoBehaviour
    {
        public event Action<Vector3> OnVictory;
        
        private ICombatant _owner;
        private IEnemyDetector _enemyDetector;
        private ICombatResolver _combatResolver;
        
        private float _combatCooldown = 0.5f;
        private float _lastCombatTime = -999f;
        
        public void Initialize(UnitController controller, CombatHierarchy combatHierarchy, UnitsManager unitsManager)
        {
            if (combatHierarchy == null)
            {
                Debug.LogError("Combat Hierarchy not assigned! Please assign it in the inspector.");
                return;
            }
            
            _owner = controller.Unit;
            _enemyDetector = new CombatDetector(
                transform, 
                _owner.CombatLayer, 
                _owner.DetectionRadius, 
                unitsManager
            );
            _combatResolver = new CombatResolver(combatHierarchy);
        }
        
        private void Update()
        {
            
            if (_enemyDetector == null || _owner == null) return;
            if (!_owner.CanEngageCombat()) return;
            
            // Cooldown check
            if (Time.time - _lastCombatTime < _combatCooldown) return;
            
            ICombatant enemy = _enemyDetector.FindNearestEnemy();
            if (enemy == null) return;
            
            TryEngageCombat(enemy);
        }
        
        private void TryEngageCombat(ICombatant  enemy)
        {
            if (!enemy.CanEngageCombat()) return;
            _lastCombatTime = Time.time;
            
            CombatResult result = _combatResolver.ResolveCombat(_owner, enemy);
            if (result == null) return;
            
            result.Apply();
            OnVictory?.Invoke(result.Winner.GameObject.transform.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _owner.DetectionRadius);
        }
    }
#endif
}