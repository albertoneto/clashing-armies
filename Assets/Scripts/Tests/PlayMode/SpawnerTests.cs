using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ClashingArmies.Units;

namespace ClashingArmies.Tests
{
    public class SpawnerTests : PoolingSystemTestBase
    {
        private GameObject spawnerObject;
        private Spawner spawner;
        private GameObject unitsManagerObject;
        private UnitsManager unitsManager;

        [SetUp]
        public void SetUp()
        {
            SetupPoolingSystem(10);
        
            unitsManagerObject = new GameObject("UnitsManager");
            unitsManager = unitsManagerObject.AddComponent<UnitsManager>();
        
            spawnerObject = new GameObject("Spawner");
            spawner = spawnerObject.AddComponent<Spawner>();
            SetupSpawner();
        }

        [TearDown]
        public void TearDown()
        {
            if (spawner != null && spawnerObject != null) spawner.StopSpawning();
            if (spawnerObject != null) Object.DestroyImmediate(spawnerObject);
            if (unitsManagerObject != null) Object.DestroyImmediate(unitsManagerObject);
            TearDownPoolingSystem();
        }

        private void SetupSpawner()
        {
            var unitsField = typeof(Spawner).GetField("units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = new System.Collections.Generic.List<UnitData>();
            var redUnit = CreateUnit(UnitType.Red);
            unitsList.Add(redUnit.data);
            var blueUnit = CreateUnit(UnitType.Blue);
            unitsList.Add(blueUnit.data);
            unitsField.SetValue(spawner, unitsList);
            
            var timeBetweenSpawnsField = typeof(Spawner).GetField("timeBetweenSpawns",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            timeBetweenSpawnsField.SetValue(spawner, 0.1f);
            
            var spawnOnStartField = typeof(Spawner).GetField("spawnOnStart",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            spawnOnStartField.SetValue(spawner, false);
        }

        private static Unit CreateUnit(UnitType type)
        {
            Unit redUnit = new Unit();
            redUnit.data = ScriptableObject.CreateInstance<UnitData>();
            redUnit.data.unitType = type;
            return redUnit;
        }

        [Test]
        public void Initialize_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => spawner.Initialize(poolingSystem, unitsManager));
        }

        [UnityTest]
        public IEnumerator StartSpawning_ShouldSpawnUnits()
        {
            spawner.Initialize(poolingSystem, unitsManager);
            int initialCount = unitsManager.GetUnitCount();
            
            spawner.StartSpawning();
            yield return new WaitForSeconds(0.1f);
            spawner.StopSpawning();
            
            int finalCount = unitsManager.GetUnitCount();
            Assert.Greater(finalCount, initialCount, $"Initial: {initialCount}, Final: {finalCount}");
        }

        [Test]
        public void StopSpawning_ShouldNotThrowException()
        {
            spawner.Initialize(poolingSystem, unitsManager);
            spawner.StartSpawning();
            
            Assert.DoesNotThrow(() => spawner.StopSpawning());
        }

        [UnityTest]
        public IEnumerator SpawnUnit_ShouldCreateUnitWithCorrectType()
        {
            spawner.Initialize(poolingSystem, unitsManager);
            int initialCount = unitsManager.GetUnitCount();
            
            Unit unit = CreateUnit(UnitType.Red);
            spawner.SpawnUnit(unit.data);
            yield return null;
            
            int finalCount = unitsManager.GetUnitCount();
            Assert.AreEqual(initialCount + 1, finalCount, "Should have added exactly 1 unit");
            
            Unit lastUnit = unitsManager.GetLastUnit();
            Assert.IsNotNull(lastUnit, "Last unit should not be null");
            Assert.AreEqual(UnitType.Red, lastUnit.data.unitType, "Incorrect unit type");
        }

        [UnityTest]
        public IEnumerator MaxUnitsToSpawn_ShouldStopAfterLimit()
        {
            var maxUnitsField = typeof(Spawner).GetField("maxUnitsToSpawn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            maxUnitsField.SetValue(spawner, 2);
            
            spawner.Initialize(poolingSystem, unitsManager);
            spawner.StartSpawning();
            yield return new WaitForSeconds(0.35f);
            
            int unitCount = unitsManager.GetUnitCount();
            Assert.LessOrEqual(unitCount, 2, $"It shouldn't spawn more than 2 units. Spawned: {unitCount}");
        }

        [Test]
        public void StartSpawning_WithNoUnits_ShouldLogWarning()
        {
            var unitsField = typeof(Spawner).GetField("units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            unitsField.SetValue(spawner, new System.Collections.Generic.List<UnitData>());
            
            spawner.Initialize(poolingSystem, unitsManager);
            
            LogAssert.Expect(LogType.Warning, $"[Spawner] {spawner.gameObject.name} has no units to spawn!");
            spawner.StartSpawning();
        }
    }
}