using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatDetector : IEnemyDetector
    {
        private readonly Transform _transform;
        private readonly int _ownerLayer;
        private readonly float _detectionRadius;
        private readonly UnitsManager _unitsManager;
        private readonly Collider[] _colliderBuffer = new Collider[1];
        
        public CombatDetector(Transform transform, int ownerLayer, float detectionRadius, UnitsManager unitsManager)
        {
            _transform = transform;
            _ownerLayer = ownerLayer;
            _detectionRadius = detectionRadius;
            _unitsManager = unitsManager;
        }
        
        public ICombatant FindFirst()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                _transform.position, 
                _detectionRadius, 
                _colliderBuffer
            );
            
            if (hitCount == 0) return null;
            GameObject hitObject = _colliderBuffer[0].gameObject;
            
            if (hitObject == _transform.gameObject) return null;
            if (Physics.GetIgnoreLayerCollision(_ownerLayer, hitObject.layer)) return null;
            Debug.Log($"{_transform.name} vs {hitObject.name}");

            return _unitsManager.GetUnit(hitObject);
        }

        public bool InRange()
        {
            return FindFirst() != null;
        }
    }
}