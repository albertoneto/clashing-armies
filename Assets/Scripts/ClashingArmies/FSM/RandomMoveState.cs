using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class RandomMoveState : MoveState
    {
        private Vector3 _targetPosition;

        public RandomMoveState(Unit unit) : base(unit)
        {
            SetNewTarget();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            SetNewTarget();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (_unitTransform == null) return;
            
            if (_timer >= _unit.data.changeTargetTime)
            {
                SetNewTarget();
                _timer = 0f;
            }

            Vector3 direction = (_targetPosition - _unitTransform.position).normalized;
            _unitTransform.position += direction * _unit.data.speed * Time.deltaTime;
        }

        private void SetNewTarget()
        {
            var offset = _unit.data.randomOffset;
            Vector3 randomOffset = new Vector3(
                Random.Range(-offset.x, offset.x),
                Random.Range(-offset.y, offset.y),
                Random.Range(-offset.z, offset.z));
            _targetPosition = _unitTransform.position + randomOffset;
        }
    }
}