using System.Collections.Generic;
using ClashingArmies.Combat;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField]
        private PoolingSystem poolingSystem;
        [SerializeField]
        private List<Spawner> spawners;
        [SerializeField]
        private UnitsManager unitsManager;
        [SerializeField]
        private CombatHierarchy combatHierarchy;

        private void Start()
        {
            foreach (var spawner in spawners)
            {
                spawner.Initialize(poolingSystem, unitsManager, combatHierarchy);
            }
        }
    }
}