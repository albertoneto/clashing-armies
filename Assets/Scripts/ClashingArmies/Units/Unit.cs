using ClashingArmies.Health;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class Unit
    {
        public string id;
        public GameObject UnitObject;
        public UnitData data;
        public UnitController controller;
        public UnitView view;
        public HealthSystem health;

        public void Dispose()
        {
            health.OnDeath -= controller.HandleDeath;
        }
    }
}