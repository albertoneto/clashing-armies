using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class RandomMoveState : MoveState
    {
        private Vector3 _targetPosition;
        private const float ReachThresholdSqr = 0.01f;

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
            
            if (UnitTransform == null) return;
            
            if (Timer >= Unit.data.changeTargetTime)
            {
                SetNewTarget();
                Timer = 0f;
            }
            
            Vector3 toTarget = _targetPosition - UnitTransform.position;
            float distanceSqr = toTarget.sqrMagnitude;
            
            if (distanceSqr < ReachThresholdSqr)
            {
                SetNewTarget();
                Timer = 0f;
                return;
            }
            
            float moveDistance = Unit.data.speed * Time.deltaTime;
            
            if (moveDistance * moveDistance > distanceSqr)
            {
                UnitTransform.position = _targetPosition;
            }
            else
            {
                Vector3 direction = toTarget.normalized;
                UnitTransform.position += direction * moveDistance;
            }
        }

        private void SetNewTarget()
        {
            var offset = Unit.data.randomOffset;
            Vector3 randomOffset = new Vector3(
                Random.Range(-offset.x, offset.x),
                Random.Range(-offset.y, offset.y),
                Random.Range(-offset.z, offset.z));
            _targetPosition = UnitTransform.position + randomOffset;
        }
    }
}