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
        public void AddUnit_ShouldAddUnitToList()
        {
            Unit unit = CreateUnit(UnitType.Red);
            unitsManager.AddUnit(unit);

            var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = unitsField.GetValue(unitsManager) as System.Collections.Generic.List<Unit>;

            Assert.IsNotNull(unitsList);
            Assert.AreEqual(1, unitsList.Count);
            Assert.AreEqual(UnitType.Red, unitsList[0].data.unitType);
        }

        [Test]
        public void AddUnit_MultipleUnits_ShouldAddAllToList()
        {
            Unit unit1 = CreateUnit(UnitType.Red);
            Unit unit2 = CreateUnit(UnitType.Blue);
            Unit unit3 = CreateUnit(UnitType.Green);

            unitsManager.AddUnit(unit1);
            unitsManager.AddUnit(unit2);
            unitsManager.AddUnit(unit3);

            var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = unitsField.GetValue(unitsManager) as System.Collections.Generic.List<Unit>;

            Assert.IsNotNull(unitsList);
            Assert.AreEqual(3, unitsList.Count);
            Assert.AreEqual(UnitType.Red, unitsList[0].data.unitType);
            Assert.AreEqual(UnitType.Blue, unitsList[1].data.unitType);
            Assert.AreEqual(UnitType.Green, unitsList[2].data.unitType);
        }

        private static Unit CreateUnit(UnitType type)
        {
            Unit redUnit = new Unit();
            redUnit.data = ScriptableObject.CreateInstance<UnitData>();
            redUnit.data.unitType = type;
            return redUnit;
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