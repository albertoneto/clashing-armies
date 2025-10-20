using ClashingArmies.Combat;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public abstract class MoveState : IState
    {
        protected readonly Unit Unit;
        protected readonly Transform UnitTransform;
        protected float Timer;
        
        private readonly CombatSystem _combatSystem;

        protected MoveState(Unit unit)
        {
            Unit = unit;
            UnitTransform = Unit.UnitObject.transform;
            _combatSystem = Unit.controller.combatSystem;
        }

        public virtual void OnEnter() 
        {
            Timer = 0f;
            
        }
        public virtual void OnExit() { }
        public virtual void OnUpdate() 
        {
            if (UnitTransform == null) return;

            Timer += Time.deltaTime;
            _combatSystem.Tick();
        }
        public virtual void OnFixedUpdate() { }
    }
}