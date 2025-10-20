using System;
using ClashingArmies.Combat;
using UnityEngine;

namespace ClashingArmies.Units
{
    [RequireComponent(typeof(CombatSystem))]
    public class UnitController
    {
        public CombatSystem combatSystem;
        public StateMachine stateMachine;
        
        private Unit _unit;
        private PoolingSystem _poolingSystem;
        private UnitsManager _unitsManager;

        public Unit Unit => _unit;
        
        public UnitController(Unit unit, PoolingSystem poolingSystem, UnitsManager unitsManager)
        {
            _unit = unit;
            stateMachine = _unit.UnitObject.AddComponent<StateMachine>();
            _poolingSystem = poolingSystem;
            _unitsManager = unitsManager;
            
            InitializeStates();
            SetInitialState();
        }

        private void InitializeStates()
        {
            stateMachine.AddState(new PatrolState(_unit));
            stateMachine.AddState(new RandomMoveState(_unit));
            stateMachine.AddState(new CombatState(_unit));
        }
        
        private void SetInitialState()
        {
            switch (_unit.data.initialState)
            {
                default:
                case UnitData.InitialStateType.Randomly:
                    stateMachine.SetState<RandomMoveState>();
                    break;
                case UnitData.InitialStateType.Patrol:
                    stateMachine.SetState<PatrolState>();
                    break;
            }
        }
        
        public void HandleDeath()
        {
            _unit.health.ResetHealth();
            _unit.health.OnDeath -= HandleDeath;
            _unitsManager.RemoveUnit(_unit);
            
            if (_poolingSystem == null || _unit?.UnitObject == null) return;
            _poolingSystem.ReturnToPool(PoolingSystem.PoolType.Unit, _unit.UnitObject);
        }
    }
}