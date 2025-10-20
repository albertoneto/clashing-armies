using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class PatrolState : MoveState
    {
        private readonly Vector3[] _waypoints;
        private readonly float _waypointReachDistance = 0.1f;
        
        private int _currentWaypointIndex;

        public PatrolState(Unit unit) : base(unit) 
        {
            Vector3[] localWaypoints = unit.data.waypoints;
            _waypoints = new Vector3[localWaypoints.Length];
            
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                _waypoints[i] = unit.UnitObject.transform.position + localWaypoints[i];
            }
            
            _currentWaypointIndex = 0;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (_waypoints == null || _waypoints.Length == 0)
            {
                Debug.LogWarning($"[{Unit.UnitObject.name}] PatrolState: No waypoints defined!");
                return;
            }

            _currentWaypointIndex = FindNearestWaypointIndex();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_waypoints == null || _waypoints.Length == 0) return;

            Vector3 targetWaypoint = _waypoints[_currentWaypointIndex];
            MoveTowards(targetWaypoint);

            if (HasReachedWaypoint(targetWaypoint))
            {
                MoveToNextWaypoint();
            }
        }

        private void MoveTowards(Vector3 target)
        {
            Vector3 direction = (target - UnitTransform.position).normalized;
            UnitTransform.position += direction * Unit.data.speed * Time.deltaTime;

            if (direction == Vector3.zero) return;
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            UnitTransform.rotation = Quaternion.Slerp(UnitTransform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        private bool HasReachedWaypoint(Vector3 waypoint)
        {
            float distance = Vector3.Distance(UnitTransform.position, waypoint);
            return distance <= _waypointReachDistance;
        }

        private void MoveToNextWaypoint()
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }

        private int FindNearestWaypointIndex()
        {
            int nearestIndex = 0;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < _waypoints.Length; i++)
            {
                float distance = Vector3.Distance(UnitTransform.position, _waypoints[i]);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }
    }
}