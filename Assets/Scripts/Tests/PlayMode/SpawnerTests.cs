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
        private UnitsManagerTestable unitsManager;

        [SetUp]
        public void SetUp()
        {
            SetupPoolingSystem(10);
        
            unitsManagerObject = new GameObject("UnitsManager");
            unitsManager = unitsManagerObject.AddComponent<UnitsManagerTestable>();
        
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
            var unitsField = typeof(Spawner).GetField("units",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = new System.Collections.Generic.List<UnitsManager.UnitType>
            {
                UnitsManager.UnitType.Red,
                UnitsManager.UnitType.Blue
            };
            unitsField.SetValue(spawner, unitsList);
            
            var timeBetweenSpawnsField = typeof(Spawner).GetField("timeBetweenSpawns",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            timeBetweenSpawnsField.SetValue(spawner, 0.1f);
            
            var spawnOnStartField = typeof(Spawner).GetField("spawnOnStart",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            spawnOnStartField.SetValue(spawner, false);
        }

        [Test]
        public void Initialize_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => spawner.Initialize(poolingSystem, unitsManager));
        }

        [UnityTest]
        public IEnumerator StartSpawning_ShouldSpawnUnits()
        {
            // Arrange
            spawner.Initialize(poolingSystem, unitsManager);
            int initialCount = unitsManager.GetUnitCount();
            
            // Act
            spawner.StartSpawning();
            
            // Espera spawnar pelo menos 1 unidade
            yield return new WaitForSeconds(0.15f);
            
            // Para o spawning para não continuar
            spawner.StopSpawning();
            
            // Assert
            int finalCount = unitsManager.GetUnitCount();
            Assert.Greater(finalCount, initialCount, 
                $"Esperava spawnar unidades. Inicial: {initialCount}, Final: {finalCount}");
        }

        [Test]
        public void StopSpawning_ShouldNotThrowException()
        {
            // Arrange
            spawner.Initialize(poolingSystem, unitsManager);
            spawner.StartSpawning();
            
            // Act & Assert
            Assert.DoesNotThrow(() => spawner.StopSpawning());
        }

        [UnityTest]
        public IEnumerator SpawnUnit_ShouldCreateUnitWithCorrectType()
        {
            // Arrange
            spawner.Initialize(poolingSystem, unitsManager);
            int initialCount = unitsManager.GetUnitCount();
            
            // Act
            spawner.SpawnUnit(UnitsManager.UnitType.Red);
            yield return null;
            
            // Assert
            int finalCount = unitsManager.GetUnitCount();
            Assert.AreEqual(initialCount + 1, finalCount, "Deveria ter adicionado exatamente 1 unidade");
            
            Unit lastUnit = unitsManager.GetLastUnit();
            Assert.IsNotNull(lastUnit, "Última unidade não deveria ser null");
            Assert.AreEqual(UnitsManager.UnitType.Red, lastUnit.type, "Tipo da unidade incorreto");
        }

        [UnityTest]
        public IEnumerator MaxUnitsToSpawn_ShouldStopAfterLimit()
        {
            // Arrange
            var maxUnitsField = typeof(Spawner).GetField("maxUnitsToSpawn",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            maxUnitsField.SetValue(spawner, 2);
            
            spawner.Initialize(poolingSystem, unitsManager);
            
            // Act
            spawner.StartSpawning();
            
            // Espera tempo suficiente para spawnar as unidades
            yield return new WaitForSeconds(0.35f);
            
            // Assert
            int unitCount = unitsManager.GetUnitCount();
            Assert.LessOrEqual(unitCount, 2, $"Não deveria spawnar mais que 2 unidades. Spawnou: {unitCount}");
        }

        [Test]
        public void StartSpawning_WithNoUnits_ShouldLogWarning()
        {
            // Arrange
            var unitsField = typeof(Spawner).GetField("units",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            unitsField.SetValue(spawner, new System.Collections.Generic.List<UnitsManager.UnitType>());
            
            spawner.Initialize(poolingSystem, unitsManager);
            
            // Act & Assert
            LogAssert.Expect(LogType.Warning, $"[Spawner] {spawner.gameObject.name} has no units to spawn!");
            spawner.StartSpawning();
        }
    }

    /// <summary>
    /// Versão testável do UnitsManager que expõe métodos para verificação
    /// </summary>
    public class UnitsManagerTestable : UnitsManager
    {
        private System.Collections.Generic.List<Unit> testUnits = new System.Collections.Generic.List<Unit>();

        private void Awake()
        {
            // Inicializa a lista privada usando reflection
            var unitsField = typeof(UnitsManager).GetField("_units",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            unitsField.SetValue(this, testUnits);
        }

        public int GetUnitCount()
        {
            return testUnits.Count;
        }

        public Unit GetLastUnit()
        {
            return testUnits.Count > 0 ? testUnits[testUnits.Count - 1] : null;
        }

        public System.Collections.Generic.List<Unit> GetAllUnits()
        {
            return new System.Collections.Generic.List<Unit>(testUnits);
        }
    }
}