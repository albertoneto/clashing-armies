using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitController : MonoBehaviour
    {
        public Unit Unit;
        private StateMachine _stateMachine;

        public void Initialize(Unit unit, StateMachine stateMachine)
        {
            Unit = unit;
            _stateMachine = stateMachine;
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            _stateMachine.AddState(new PatrolState(Unit));
            _stateMachine.AddState(new RandomMoveState(Unit));
            _stateMachine.AddState(new CombatState());

            switch (Unit.data.InitialState)
            {
                default:
                case UnitData.InitialStateType.Randomly:
                    _stateMachine.SetState<RandomMoveState>();
                    break;
                case UnitData.InitialStateType.Patrol:
                    _stateMachine.SetState<PatrolState>();
                    break;
            }
        }

        public void StartCombat()
        {
            Debug.Log(Unit.UnitObject.name);
            _stateMachine.SetState<CombatState>();
        }
    }
}