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
            Unit unit = new Unit { type = UnitsManager.UnitType.Red };
            unitsManager.AddUnit(unit);

            var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = unitsField.GetValue(unitsManager) as System.Collections.Generic.List<Unit>;

            Assert.IsNotNull(unitsList);
            Assert.AreEqual(1, unitsList.Count);
            Assert.AreEqual(UnitsManager.UnitType.Red, unitsList[0].type);
        }

        [Test]
        public void AddUnit_MultipleUnits_ShouldAddAllToList()
        {
            Unit unit1 = new Unit { type = UnitsManager.UnitType.Red };
            Unit unit2 = new Unit { type = UnitsManager.UnitType.Blue };
            Unit unit3 = new Unit { type = UnitsManager.UnitType.Green };

            unitsManager.AddUnit(unit1);
            unitsManager.AddUnit(unit2);
            unitsManager.AddUnit(unit3);

            var unitsField = typeof(UnitsManager).GetField("_units", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var unitsList = unitsField.GetValue(unitsManager) as System.Collections.Generic.List<Unit>;

            Assert.IsNotNull(unitsList);
            Assert.AreEqual(3, unitsList.Count);
            Assert.AreEqual(UnitsManager.UnitType.Red, unitsList[0].type);
            Assert.AreEqual(UnitsManager.UnitType.Blue, unitsList[1].type);
            Assert.AreEqual(UnitsManager.UnitType.Green, unitsList[2].type);
        }

        [Test]
        public void UnitType_ShouldHaveAllExpectedValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitsManager.UnitType), UnitsManager.UnitType.Red));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitsManager.UnitType), UnitsManager.UnitType.Green));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitsManager.UnitType), UnitsManager.UnitType.Blue));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UnitsManager.UnitType), UnitsManager.UnitType.Yellow));
        }
    }
}