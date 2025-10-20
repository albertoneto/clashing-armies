using System;
using System.Collections;
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
        private StateMachine _stateMachine;
        private CombatSettings _combatSettings;
        private float _lastCombatTime = -999f;
        
        public void Initialize(UnitController controller, CombatSettings combatSettings, UnitsManager unitsManager)
        {
            if (combatSettings == null)
            {
                Debug.LogError("Combat Hierarchy not assigned! Please assign it in the inspector.");
                return;
            }
            
            _owner = controller.Unit;
            _combatSettings = combatSettings;
            _stateMachine = controller.stateMachine;
            _enemyDetector = new CombatDetector(transform, _owner.CombatLayer, _owner.DetectionRadius, unitsManager);
            _combatResolver = new CombatResolver(combatSettings);
        }
        
        private void Update()
        {
            if (_enemyDetector == null || _owner == null) return;
            if (!_owner.CanEngageCombat()) return;
            if (IsInCombatState()) return;    
            if (Time.time - _lastCombatTime < _combatSettings.combatCooldown) return;
            
            ICombatant enemy = _enemyDetector.FindFirst();
            if (enemy == null) return;
            if (!enemy.CanEngageCombat()) return;

            EnterCombat(enemy);
        }
        
        private bool IsInCombatState()
        {
            if (_stateMachine == null) return false;
            return _stateMachine.CurrentState is CombatState;
        }

        private void EnterCombat(ICombatant enemy)
        {
            if (_stateMachine == null) return;
            
            CombatResult result = _combatResolver.ResolveCombat(_owner, enemy);
            if (result == null) return;
    
            _stateMachine.SetState<CombatState>();
            enemy.Controller.stateMachine.SetState<CombatState>();
            StartCoroutine(WaitAndResolveCombat(result));
        }

        private IEnumerator WaitAndResolveCombat(CombatResult result)
        {
            yield return new WaitForSeconds(_combatSettings.combatDuration);

            _lastCombatTime = Time.time;
            result.Apply();
            OnVictory?.Invoke(result.Winner.GameObject.transform.position);
        }

        public bool HasNearby()
        {
            return _enemyDetector?.InRange() ?? false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _owner.DetectionRadius);
        }
#endif
    }
}