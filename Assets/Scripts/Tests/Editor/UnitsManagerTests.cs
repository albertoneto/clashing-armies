#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using ClashingArmies.Units;

namespace ClashingArmies.Tests
{
    public class UnitsManagerTests
    {
        private GameObject unitsManagerObject;
        private UnitsManager unitsManager;

        [SetUp]
        public void SetUp()
        {
            unitsManagerObject = new GameObject("UnitsManager");
            unitsManager = unitsManagerObject.AddComponent<UnitsManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (unitsManagerObject != null)
            {
                Object.DestroyImmediate(unitsManagerObject);
            }
        }

        [Test]
        public void AddUnit_ShouldAddUnit()
        {
            Unit unit = CreateUnit(UnitType.Red);
            unitsManager.AddUnit(unit);

            var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsDict = unitsField.GetValue(unitsManager) as System.Collections.Generic.Dictionary<GameObject, Unit>;

            Assert.IsNotNull(unitsDict);
            Assert.AreEqual(1, unitsDict.Count);
            Assert.AreEqual(UnitType.Red, unit.data.unitType);
        }

        [Test]
        public void AddUnit_MultipleUnits_ShouldAddAll()
        {
            Unit unit1 = CreateUnit(UnitType.Red);
            Unit unit2 = CreateUnit(UnitType.Blue);
            Unit unit3 = CreateUnit(UnitType.Green);

            unitsManager.AddUnit(unit1);
            unitsManager.AddUnit(unit2);
            unitsManager.AddUnit(unit3);

            var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsDict = unitsField.GetValue(unitsManager) as System.Collections.Generic.Dictionary<GameObject, Unit>;

            Assert.IsNotNull(unitsDict);
            Assert.AreEqual(3, unitsDict.Count);
        }

        private static Unit CreateUnit(UnitType type)
        {
            Unit unit = new Unit();
            unit.UnitObject = new GameObject($"Unit_{type}"); // ADICIONADO: Cria GameObject
            unit.data = ScriptableObject.CreateInstance<UnitData>();
            unit.data.unitType = type;
            return unit;
        }

        [Test]
        public void UnitType_ShouldHaveAllExpectedValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitType), UnitType.Red));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitType), UnitType.Green));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitType), UnitType.Blue));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitType), UnitType.Yellow));
        }
    }
}
#endif