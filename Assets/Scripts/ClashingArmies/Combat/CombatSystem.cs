using System;
using System.Collections;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatSystem
    {
        public event Action<Vector3> OnVictory;
        
        private ICombatant _owner;
        private IEnemyDetector _enemyDetector;
        private ICombatResolver _combatResolver;
        private StateMachine _stateMachine;
        private CombatSettings _combatSettings;
        private WaitForSeconds _waitCombatDuration;
        
        public CombatSystem(UnitController controller, CombatSettings combatSettings, UnitsManager unitsManager)
        {
            if (combatSettings == null)
            {
                Debug.LogError("Combat Hierarchy not assigned! Please assign it in the inspector.");
                return;
            }
            
            _owner = controller.Unit;
            _combatSettings = combatSettings;
            _stateMachine = controller.stateMachine;
            _enemyDetector = new CombatDetector(_owner.GameObject.transform, _owner.CombatLayer, _owner.DetectionRadius, unitsManager);
            _combatResolver = new CombatResolver(combatSettings);
            _waitCombatDuration = new WaitForSeconds(_combatSettings.combatDuration);
        }
        
        public void Tick()
        {
            if (!CanStartCombat()) return;
            if (!CanEnemyEngage(out var enemy)) return;

            EnterCombat(enemy);
        }

        private bool CanStartCombat()
        {
            if (_enemyDetector == null || _owner == null) return false;
            if (_stateMachine == null) return false;
            if (_stateMachine.CurrentState is CombatState) return false;    
            if (!_owner.CanEngageCombat()) return false;
            
            return true;
        }

        private bool CanEnemyEngage(out ICombatant enemy)
        {
            enemy = _enemyDetector.FindFirst();
            if (enemy == null) return false;
            if (enemy.Controller.stateMachine.CurrentState is CombatState) return false;
            if (!enemy.CanEngageCombat()) return false;
            
            return true;
        }

        private void EnterCombat(ICombatant enemy)
        {
            CombatResult result = _combatResolver.ResolveCombat(_owner, enemy);
            if (result == null) return;
    
            _stateMachine.SetState<CombatState>();
            enemy.Controller.stateMachine.SetState<CombatState>();
            _owner.Controller.stateMachine.StartCoroutine(WaitAndResolveCombat(result));
        }

        private IEnumerator WaitAndResolveCombat(CombatResult result)
        {
            yield return _waitCombatDuration;

            result.Apply();
            result.Winner.Controller.combatSystem.OnVictory?.Invoke(result.Winner.GameObject.transform.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_owner == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_owner.GameObject.transform.position, _owner.DetectionRadius);
        }
#endif
    }
}