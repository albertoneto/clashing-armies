using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class RandomMoveState : IState
    {
        private readonly string _stateName;
        private readonly Transform _unitTransform;
        
        private Unit _unit;
        private Vector3 _targetPosition;
        private float _timer;

        public RandomMoveState(string name, Unit unit)
        {
            _stateName = name;
            _unit = unit;
            _unitTransform = _unit.UnitObject.transform;
            
            SetNewTarget();
        }

        public void OnEnter()
        {
            _timer = 0f;
            SetNewTarget();
        }

        public void OnExit()
        {
        }

        public void OnUpdate()
        {
            if (_unitTransform == null) return;

            _timer += Time.deltaTime;
            if (_timer >= _unit.data.ChangeTargetTime)
            {
                SetNewTarget();
                _timer = 0f;
            }

            Vector3 direction = (_targetPosition - _unitTransform.position).normalized;
            _unitTransform.position += direction * _unit.data.Speed * Time.deltaTime;
        }

        public void OnFixedUpdate()
        {
        }

        public string GetStateName() => _stateName;

        private void SetNewTarget()
        {
            Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
            _targetPosition = _unitTransform.position + randomOffset;
        }
    }
}