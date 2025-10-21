#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ClashingArmies.Units;
using ClashingArmies.Combat;

namespace ClashingArmies.Tests
{
    public class SpawnerTests : PoolingSystemTestBase
    {
        private GameObject spawnerObject;
        private Spawner spawner;
        private GameObject unitsManagerObject;
        private UnitsManager unitsManager;
        private CombatSettings combatSettings;

        [SetUp]
        public void SetUp()
        {
            SetupPoolingSystem(10);
        
            unitsManagerObject = new GameObject("UnitsManager");
            unitsManager = unitsManagerObject.AddComponent<UnitsManager>();

            combatSettings = ScriptableObject.CreateInstance<CombatSettings>();
            combatSettings.combatDuration = 1f;
            combatSettings.combatLayer = LayerMask.GetMask("Default");
            combatSettings.unitStrengths = new System.Collections.Generic.List<CombatSettings.UnitStrength>
            {
                new CombatSettings.UnitStrength { unitType = UnitType.Red, StrengthLevel = 5 },
                new CombatSettings.UnitStrength { unitType = UnitType.Blue, StrengthLevel = 3 }
            };
        
            spawnerObject = new GameObject("Spawner");
            spawner = spawnerObject.AddComponent<Spawner>();
            SetupSpawner();
        }

        [TearDown]
        public void TearDown()
        {
            if (spawner != null && spawnerObject != null) spawner.StopSpawning();
    
            if (unitsManager != null)
            {
                var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var unitsDict = unitsField.GetValue(unitsManager) as System.Collections.Generic.Dictionary<GameObject, Unit>;
                if (unitsDict != null)
                {
                    var unitsList = new System.Collections.Generic.List<Unit>(unitsDict.Values);
                    foreach (var unit in unitsList)
                    {
                        if (unit?.UnitObject != null) Object.DestroyImmediate(unit.UnitObject);
                    }
                }
            }
    
            if (spawnerObject != null) Object.DestroyImmediate(spawnerObject);
            if (unitsManagerObject != null) Object.DestroyImmediate(unitsManagerObject);
            if (combatSettings != null) Object.DestroyImmediate(combatSettings);
            TearDownPoolingSystem();
        }

        private void SetupSpawner()
        {
            var unitsField = typeof(Spawner).GetField("units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = new System.Collections.Generic.List<UnitData>();
            
            var redUnitData = CreateMockedUnit();
            unitsList.Add(redUnitData);
            
            var blueUnitData = CreateMockedUnit(UnitType.Blue);
            unitsList.Add(blueUnitData);
            
            unitsField.SetValue(spawner, unitsList);
            
            var timeBetweenSpawnsField = typeof(Spawner).GetField("timeBetweenSpawns",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            timeBetweenSpawnsField.SetValue(spawner, 0.1f);
            
            var spawnOnStartField = typeof(Spawner).GetField("spawnOnStart",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            spawnOnStartField.SetValue(spawner, false);
        }

        private static UnitData CreateMockedUnit(UnitType unitType = UnitType.Red)
        {
            var redUnitData = ScriptableObject.CreateInstance<UnitData>();
            redUnitData.unitType = unitType;
            redUnitData.maxHealth = 100f;
            redUnitData.layer = LayerMask.GetMask(unitType.ToString().ToLower());
            redUnitData.detectionRadius = 2f;
            redUnitData.speed = 2f;
            redUnitData.waypoints = new[] { Vector3.right, -Vector3.right };
            return redUnitData;
        }

        [Test]
        public void Initialize_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => spawner.Initialize(poolingSystem, unitsManager, combatSettings));
        }

        [Test]
        public void Initialize_WithNullCombatSettings_ShouldWork()
        {
            Assert.DoesNotThrow(() => spawner.Initialize(poolingSystem, unitsManager, null));
        }

        [UnityTest]
        public IEnumerator StartSpawning_ShouldSpawnUnits()
        {
            spawner.Initialize(poolingSystem, unitsManager, combatSettings);
            int initialCount = unitsManager.GetUnitCount();
            
            spawner.StartSpawning();
            yield return new WaitForSeconds(0.25f);
            spawner.StopSpawning();
            
            int finalCount = unitsManager.GetUnitCount();
            Assert.Greater(finalCount, initialCount, $"Should spawn units. Initial: {initialCount}, Final: {finalCount}");
        }

        [Test]
        public void StopSpawning_ShouldNotThrowException()
        {
            spawner.Initialize(poolingSystem, unitsManager, combatSettings);
            spawner.StartSpawning();
            
            Assert.DoesNotThrow(() => spawner.StopSpawning());
        }

        [Test]
        public void StopSpawning_WhenNotSpawning_ShouldNotThrow()
        {
            spawner.Initialize(poolingSystem, unitsManager, combatSettings);
            
            Assert.DoesNotThrow(() => spawner.StopSpawning());
        }

        [UnityTest]
        public IEnumerator SpawnUnit_ShouldCreateUnitWithCorrectType()
        {
            spawner.Initialize(poolingSystem, unitsManager, combatSettings);
            int initialCount = unitsManager.GetUnitCount();
            
            var unitData = CreateMockedUnit();
            
            spawner.SpawnUnit(unitData);
            yield return null;
            
            int finalCount = unitsManager.GetUnitCount();
            Assert.AreEqual(initialCount + 1, finalCount, "Should have added exactly 1 unit");
            
            Unit lastUnit = unitsManager.GetLastUnit();
            Assert.IsNotNull(lastUnit, "Last unit should not be null");
            Assert.AreEqual(UnitType.Red, lastUnit.data.unitType, "Incorrect unit type");
            
            Object.DestroyImmediate(unitData);
        }

        [Test]
        public void StartSpawning_WithNoUnits_ShouldLogWarning()
        {
            var unitsField = typeof(Spawner).GetField("units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            unitsField.SetValue(spawner, new System.Collections.Generic.List<UnitData>());
            
            spawner.Initialize(poolingSystem, unitsManager, combatSettings);
            
            LogAssert.Expect(LogType.Warning, $"[Spawner] {spawner.gameObject.name} has no units to spawn!");
            spawner.StartSpawning();
        }

        [UnityTest]
        public IEnumerator StartSpawning_MultipleTimes_ShouldNotDuplicate()
        {
            spawner.Initialize(poolingSystem, unitsManager, combatSettings);
            
            spawner.StartSpawning();
            spawner.StartSpawning();
            
            yield return new WaitForSeconds(0.15f);
            
            spawner.StopSpawning();
            
            int count = unitsManager.GetUnitCount();
            Assert.IsTrue(count > 0 && count <= 2, $"Should spawn normally, not duplicate. Count: {count}");
        }
    }
}
#endif