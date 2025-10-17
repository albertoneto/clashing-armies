using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitBuilder
    {
        private readonly Unit _unit;

        public UnitBuilder(PoolingSystem poolingSystem, Transform parent, UnitData unitData, Vector3 spawnPosition)
        {
            GameObject unitObject = poolingSystem.SpawnFromPool(
                PoolingSystem.PoolType.Unit, 
                spawnPosition, 
                parent,
                unitData.name);
            
            if (unitObject == null)
            {
                Debug.LogError($"[UnitBuilder] Unable to get unit from pool.");
                return;
            }

            int layerIndex = Mathf.RoundToInt(Mathf.Log(unitData.layer.value, 2));
            unitObject.layer = layerIndex;
            
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
            var controller = _unit.UnitObject.AddComponent<UnitController>();
            controller.Initialize(_unit, stateMachine);
            _unit.controller = controller;
            return this;
        }

        public UnitBuilder SetCombatSystem()
        {
            var combatSystem = _unit.UnitObject.AddComponent<CombatSystem>();
            combatSystem.Initialize(_unit);
            return this;
        }

        public UnitBuilder SetHealth()
        {
            
            return this;
        }

        public Unit Build()
        {
            return _unit;
        }
    }
}