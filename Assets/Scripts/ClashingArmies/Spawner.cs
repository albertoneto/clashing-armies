using System.Collections;
using System.Collections.Generic;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] 
        private List<UnitsManager.UnitType> units;
        [SerializeField, Range(0, 100)] 
        private float timeBetweenSpawns = 2f;
        [SerializeField]
        private Transform spawnPoint;
        [SerializeField]
        private bool spawnOnStart = true;
        [SerializeField]
        private int maxUnitsToSpawn = -1;
        
        private PoolingSystem _poolingSystem;
        private UnitsManager _unitsManager;
        private Coroutine _spawnCoroutine;
        private int _spawnedUnitsCount;
        private bool _isSpawning;

        public void Initialize(PoolingSystem poolingSystem, UnitsManager unitsManager)
        {
            _poolingSystem = poolingSystem;
            _unitsManager = unitsManager;
            
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
                if (maxUnitsToSpawn > 0 && _spawnedUnitsCount >= maxUnitsToSpawn)
                {
                    StopSpawning();
                    yield break;
                }

                SpawnRandomUnit();
                _spawnedUnitsCount++;

                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        private void SpawnRandomUnit()
        {
            if (units.Count == 0) return;

            UnitsManager.UnitType unitType = units[Random.Range(0, units.Count)];
            SpawnUnit(unitType);
        }

        public void SpawnUnit(UnitsManager.UnitType unitType)
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            
            GameObject unitObject = _poolingSystem.SpawnFromPool(
                PoolingSystem.PoolType.Unit, 
                spawnPosition, 
                _unitsManager.gameObject.transform
            );
            
            if (unitObject == null)
            {
                Debug.LogError($"[Spawner] Unable to get unit from pool.");
                return;
            }

            Unit unit = new Unit
            {
                UnitType = unitType
            };

            _unitsManager.AddUnit(unit);
        }
    }
}