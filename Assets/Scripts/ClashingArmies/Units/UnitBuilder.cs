using ClashingArmies.Combat;
using ClashingArmies.Health;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitBuilder
    {
        private readonly Unit _unit;
        
        private PoolingSystem _poolingSystem;

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

            _poolingSystem = poolingSystem;
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
            renderer.material = _unit.data.material;
            return this;
        }

        public UnitBuilder SetUnitController()
        {
            var stateMachine = _unit.UnitObject.AddComponent<StateMachine>();
            var controller = _unit.UnitObject.AddComponent<UnitController>();
            controller.Initialize(_unit, stateMachine, _poolingSystem);
            _unit.controller = controller;
            return this;
        }

        public UnitBuilder SetCombatTrigger(CombatHierarchy combatHierarchy, UnitsManager unitsManager)
        {
            _unit.controller.combatSystem = _unit.UnitObject.AddComponent<CombatSystem>();
            _unit.controller.combatSystem.Initialize(_unit.controller, combatHierarchy, unitsManager);
            return this;
        }

        public UnitBuilder SetHealth()
        {
            _unit.health = new HealthSystem(_unit.data.maxHealth);
            _unit.health.OnDeath += _unit.controller.HandleDeath;
            return this;
        }

        public Unit Build()
        {
            return _unit;
        }
    }
}