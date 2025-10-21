using System;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatSystem
    {
        private const float TickCooldown = .2f;
        public event Action OnVictory;
        
        private ICombatant _owner;
        private IEnemyDetector _enemyDetector;
        private ICombatResolver _combatResolver;
        private StateMachine _stateMachine;
        private CombatSettings _combatSettings;
        
        private CombatResult _pendingResult;
        private float _combatStartTime;
        private float _lastTick;
        
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
            _enemyDetector = new CombatDetector(_owner, combatSettings, unitsManager);
            _combatResolver = new CombatResolver(combatSettings);
        }
        
        public void Tick()
        {
            _lastTick = Time.time;
            if(Time.time - _lastTick > TickCooldown) return;
            
            if (_pendingResult != null)
            {
                if (CheckCombatResolution())
                {
                    return;
                }
            }
            
            if (!CanStartCombat()) return;
            if (!CanEnemyEngage(out var enemy)) return;

            EnterCombat(enemy);
        }

        private bool CheckCombatResolution()
        {
            if (Time.time - _combatStartTime < _combatSettings.combatDuration) return false;
            
            return ResolveCombat();
        }

        private bool ResolveCombat()
        {
            if (_pendingResult == null) return false;
            
            _pendingResult.Apply();
            _pendingResult.Winner.Controller.combatSystem.OnVictory?.Invoke();
            _pendingResult = null;
            
            return true;
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

        public void SetPendingCombat(CombatResult result, float startTime)
        {
            _pendingResult = result;
            _combatStartTime = startTime;
        }

        private void EnterCombat(ICombatant enemy)
        {
            CombatResult result = _combatResolver.ResolveCombat(_owner, enemy);
            if (result == null) return;

            _stateMachine.SetState<CombatState>();
            enemy.Controller.stateMachine.SetState<CombatState>();
    
            float startTime = Time.time;
            SetPendingCombat(result, startTime);
            enemy.Controller.combatSystem.SetPendingCombat(result, startTime);
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