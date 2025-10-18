using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public abstract class MoveState : IState
    {
        protected readonly Unit _unit;
        protected readonly Transform _unitTransform;
        protected float _timer;

        protected MoveState(Unit unit)
        {
            _unit = unit;
            _unitTransform = _unit.UnitObject.transform;
        }

        public virtual void OnEnter() 
        {
            _timer = 0f;
            
        }
        public virtual void OnExit() { }
        public virtual void OnUpdate() 
        {
            if (_unitTransform == null) return;

            _timer += Time.deltaTime;
        }
        public virtual void OnFixedUpdate() { }
    }
}