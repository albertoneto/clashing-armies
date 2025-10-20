using ClashingArmies.Combat;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class CombatState : IState
    {
        private static readonly int InCombat = Animator.StringToHash("inCombat");
        private readonly Unit _unit;
        
        public CombatState(Unit unit)
        {
            _unit = unit;
        }

        public void OnEnter()
        {
            _unit.view.animator.SetBool(InCombat, true);
            _unit.controller.combatSystem.OnVictory += ReturnToMovementState;
        }

        public void OnExit()
        {
            _unit.view.animator.SetBool(InCombat, false);
            _unit.controller.combatSystem.OnVictory -= ReturnToMovementState;
        }

        public void OnUpdate() { }
        public void OnFixedUpdate() { }
        
        private void ReturnToMovementState(Vector3 position)
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