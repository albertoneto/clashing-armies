using ClashingArmies.Combat;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class CombatState : IState
    {
        private readonly Unit _unit;
        private readonly CombatSystem _combatSystem;
        
        public CombatState(Unit unit)
        {
            _unit = unit;
            _combatSystem = _unit.controller.combatSystem;
        }

        public void OnEnter()
        {
            _unit.view.SetCombatRotation(true);
            _combatSystem.OnVictory += ReturnToMovementState;
        }

        public void OnExit()
        {
            _unit.view.SetCombatRotation(false);
            _combatSystem.OnVictory -= ReturnToMovementState;
        }

        public void OnUpdate() 
        { 
            _combatSystem.Tick();
        }
        public void OnFixedUpdate() { }
        
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