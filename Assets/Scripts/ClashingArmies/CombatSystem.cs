using System;
using ClashingArmies.Health;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class CombatSystem : MonoBehaviour
    {
        private Unit _unit;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        private void OnTriggerEnter(Collider other)
        {
            _unit.controller.StartCombat();
        }
    }
}