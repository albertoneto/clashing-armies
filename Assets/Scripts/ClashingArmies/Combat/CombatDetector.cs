using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatDetector
    {
        private readonly Transform _transform;
        private readonly int _ownerLayer;
        private readonly float _detectionRadius;
        private readonly UnitsManager _unitsManager;
        
        private readonly Collider[] _colliderBuffer = new Collider[32];
        
        public CombatDetector(Transform transform, int ownerLayer, float detectionRadius, UnitsManager unitsManager)
        {
            _transform = transform;
            _ownerLayer = Mathf.RoundToInt(Mathf.Log(ownerLayer, 2));
            _detectionRadius = detectionRadius;
            _unitsManager = unitsManager;
        }
        
        public Unit FindNearestEnemy()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                _transform.position, 
                _detectionRadius, 
                _colliderBuffer
            );
            
            Unit nearestUnit = null;
            
            for (int i = 0; i < hitCount; i++)
            {
                GameObject hitObject = _colliderBuffer[i].gameObject;
                
                if (hitObject == _transform.gameObject) continue;
                if (Physics.GetIgnoreLayerCollision(_ownerLayer, hitObject.layer)) continue;

                nearestUnit = _unitsManager.GetUnit(hitObject);
            }
            
            return nearestUnit;
        }
    }
}