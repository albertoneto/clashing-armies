using System;
using ClashingArmies.Combat;
using ClashingArmies.Health;
using UnityEngine;

namespace ClashingArmies.Units
{
    [RequireComponent(typeof(CombatSystem))]
    public class UnitController : MonoBehaviour
    {
        public CombatSystem combatSystem;

        private Unit _unit;
        private StateMachine _stateMachine;
        private PoolingSystem _poolingSystem;
        
        public Unit Unit => _unit;
        
        public void Initialize(Unit unit, StateMachine stateMachine, PoolingSystem poolingSystem)
        {
            _unit = unit;
            _stateMachine = stateMachine;
            _poolingSystem = poolingSystem;
            
            InitializeStates();
            SetInitialState();
        }

        private void InitializeStates()
        {
            _stateMachine.AddState(new PatrolState(_unit));
            _stateMachine.AddState(new RandomMoveState(_unit));
            _stateMachine.AddState(new CombatState());
        }
        
        private void SetInitialState()
        {
            switch (_unit.data.initialState)
            {
                case UnitData.InitialStateType.Patrol:
                    _stateMachine.SetState<PatrolState>();
                    break;
                case UnitData.InitialStateType.Randomly:
                    _stateMachine.SetState<RandomMoveState>();
                    break;
            }
        }
        
        public void HandleDeath()
        {
            _stateMachine.enabled = false;
            combatSystem.enabled = false;
            _unit.health.ResetHealth();
            _unit.health.OnDeath -= HandleDeath;
            
            _poolingSystem.ReturnToPool(PoolingSystem.PoolType.Unit, _unit.UnitObject);
        }
    }
}