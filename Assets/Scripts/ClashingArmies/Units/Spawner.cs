using System.Collections;
using System.Collections.Generic;
using ClashingArmies.Combat;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private List<UnitData> units;
        [SerializeField, Range(0, 100)] private float timeBetweenSpawns = 2f;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool spawnOnStart = true;
        
        private PoolingSystem _poolingSystem;
        private UnitsManager _unitsManager;
        private CombatSettings _combatSettings;
        private Coroutine _spawnCoroutine;
        private int _spawnedUnitsCount;
        private bool _isSpawning;

        public void Initialize(PoolingSystem poolingSystem, UnitsManager unitsManager, CombatSettings combatSettings = null)
        {
            _poolingSystem = poolingSystem;
            _unitsManager = unitsManager;
            _combatSettings = combatSettings;
            
            if (spawnOnStart)
            {
                StartSpawning();
            }
        }

        private void OnDestroy()
        {
            StopSpawning();
        }

        private void OnDisable()
        {
            StopSpawning();
        }
        
        public void StartSpawning()
        {
            if (_isSpawning) return;
            
            if (units == null || units.Count == 0)
            {
                Debug.LogWarning($"[Spawner] {gameObject.name} has no units to spawn!");
                return;
            }

            _isSpawning = true;
            _spawnedUnitsCount = 0;
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
        }

        public void StopSpawning()
        {
            if (!_isSpawning) return;
            
            _isSpawning = false;
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (_isSpawning)
            {
                SpawnRandomUnit();
                _spawnedUnitsCount++;

                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        private void SpawnRandomUnit()
        {
            if (units.Count == 0) return;

            var unitType = units[Random.Range(0, units.Count)];
            SpawnUnit(unitType);
        }

        public void SpawnUnit(UnitData unitData)
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            
            Unit unit = new UnitBuilder(_poolingSystem, _unitsManager.gameObject.transform, unitData, spawnPosition)
                .SetId(_unitsManager.GetUnitCount().ToString())
                .SetUnitController(_unitsManager)
                .SetCombat(_combatSettings, _unitsManager)
                .SetStates()
                .SetHealth()
                .SetUnitView()
                .Build();

            _unitsManager.AddUnit(unit);
        }
    }
}