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

        public UnitBuilder SetUnitController()
        {
            _unit.controller = new UnitController(_unit, _poolingSystem);
            return this;
        }

        public UnitBuilder SetUnitView()
        {
            _unit.view = new UnitView(_unit, _poolingSystem);
            return this;
        }

        public UnitBuilder SetCombat(CombatSettings combatSettings, UnitsManager unitsManager)
        {
            _unit.controller.combatSystem = _unit.UnitObject.AddComponent<CombatSystem>();
            _unit.controller.combatSystem.Initialize(_unit.controller, combatSettings, unitsManager);
            return this;
        }

        public UnitBuilder SetHealth()
        {
            _unit.health = new HealthSystem(_unit.data.maxHealth);
            _unit.health.OnDeath += _unit.controller.HandleDeath;
            _unit.UnitObject.GetComponentInChildren<HealthBarUI>().Initialize(_unit.health);
            return this;
        }

        public Unit Build()
        {
            return _unit;
        }
    }
}