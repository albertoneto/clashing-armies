using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitController : MonoBehaviour
    {
        private Unit _unit;
        private StateMachine _stateMachine;

        public void Initialize(Unit unit, StateMachine stateMachine)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            _stateMachine.AddState(new PatrolState("Patrol", _unit));
            _stateMachine.AddState(new RandomMoveState("RandomMove", _unit));
            _stateMachine.AddState(new CombatState("Combat", _unit));

            _stateMachine.SetState("RandomMove");
        }
    }
}