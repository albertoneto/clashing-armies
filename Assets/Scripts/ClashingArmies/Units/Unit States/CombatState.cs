using ClashingArmies.Combat;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class CombatState : IState
    {
        private static readonly int InCombat = Animator.StringToHash("inCombat");
        private readonly Unit _unit;
        private readonly float _combatCheckInterval = 0.2f;
        
        private float _timeSinceLastCheck;
        
        public CombatState(Unit unit)
        {
            _unit = unit;
        }

        public void OnEnter()
        {
            _timeSinceLastCheck = 0f;
            _unit.view.animator.SetBool(InCombat, true);
        }

        public void OnExit()
        {
            _unit.view.animator.SetBool(InCombat, false);
        }

        public void OnUpdate()
        {
            _timeSinceLastCheck += Time.deltaTime;
            
            if (_timeSinceLastCheck >= _combatCheckInterval)
            {
                _timeSinceLastCheck = 0f;
                CheckForCombat();
            }
        }
        public void OnFixedUpdate() { }
        
        private void CheckForCombat()
        {
            if (_unit.controller.combatSystem.HasNearby()) return;
            ReturnToMovementState();
        }
        private void ReturnToMovementState()
        {
            var stateMachine = _unit.controller.stateMachine;
            
            switch (_unit.data.initialState)
            {
                case UnitData.InitialStateType.Patrol:
                    stateMachine.SetState<PatrolState>();
                    break;
                case UnitData.InitialStateType.Randomly:
                    stateMachine.SetState<RandomMoveState>();
                    break;
            }
        }
    }
}