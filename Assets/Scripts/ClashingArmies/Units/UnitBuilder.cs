using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitBuilder
    {
        private readonly Unit _unit;

        public UnitBuilder(PoolingSystem poolingSystem, UnitsManager unitsManager, UnitData unitData, Vector3 spawnPosition)
        {
            GameObject unitObject = poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, spawnPosition, unitsManager.gameObject.transform );
            
            if (unitObject == null)
            {
                Debug.LogError($"[UnitBuilder] Unable to get unit from pool.");
                return;
            }
            
            _unit = new Unit
            {
                data = unitData,
                UnitObject = unitObject
            };
        }

        public UnitBuilder SetId(string id)
        {
            _unit.id = id;
            return this;
        }

        public UnitBuilder SetUnitMaterial()
        {
            var renderer = _unit.UnitObject.gameObject.GetComponent<MeshRenderer>();
            renderer.material = _unit.data.Material;
            return this;
        }

        public UnitBuilder SetUnitController()
        {
            var stateMachine = _unit.UnitObject.AddComponent<StateMachine>();
            var stateController = _unit.UnitObject.AddComponent<UnitController>();
            stateController.Initialize(_unit, stateMachine);
            return this;
        }

        public UnitBuilder SetHealth(int health)
        {
            //_unit.Health = health;
            return this;
        }

        public Unit Build()
        {
            return _unit;
        }
    }
}