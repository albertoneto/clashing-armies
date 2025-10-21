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
        private readonly LayerMask _combatLayer;

        public CombatDetector(ICombatant combatant, CombatSettings combatSettings, UnitsManager unitsManager)
        {
            _transform = combatant.GameObject.transform;
            _ownerLayer = combatant.CombatLayer;
            _detectionRadius = combatant.DetectionRadius;
            _unitsManager = unitsManager;
            _combatLayer = combatSettings.combatLayer;
        }

        public ICombatant FindFirst()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                _transform.position, 
                _detectionRadius, 
                _colliderBuffer,
                _combatLayer
            );
            
            if (hitCount == 0) return null;
            GameObject hitObject = _colliderBuffer[0].gameObject;
            
            if (hitObject == _transform.gameObject) return null;
            if (Physics.GetIgnoreLayerCollision(_ownerLayer, hitObject.layer)) return null;

            return _unitsManager.GetUnit(hitObject);
        }

        public bool InRange()
        {
            return FindFirst() != null;
        }
    }
}