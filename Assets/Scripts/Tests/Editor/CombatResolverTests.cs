#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using ClashingArmies.Combat;
using ClashingArmies.Units;

namespace ClashingArmies.Tests
{
    public class CombatResolverTests
    {
        private CombatSettings combatSettings;
        private CombatResolver resolver;
        private MockCombatant redUnit;
        private MockCombatant blueUnit;

        [SetUp]
        public void SetUp()
        {
            combatSettings = ScriptableObject.CreateInstance<CombatSettings>();
            combatSettings.unitStrengths = new System.Collections.Generic.List<CombatSettings.UnitStrength>
            {
                new CombatSettings.UnitStrength { unitType = UnitType.Red, StrengthLevel = 5 },
                new CombatSettings.UnitStrength { unitType = UnitType.Blue, StrengthLevel = 3 },
                new CombatSettings.UnitStrength { unitType = UnitType.Green, StrengthLevel = 7 },
                new CombatSettings.UnitStrength { unitType = UnitType.Yellow, StrengthLevel = 1 }
            };
            combatSettings.randomWinChance = 0f;
            combatSettings.winnerDamagePercent = 10f;

            resolver = new CombatResolver(combatSettings);

            redUnit = new MockCombatant(UnitType.Red, 100f);
            blueUnit = new MockCombatant(UnitType.Blue, 100f);
        }

        [TearDown]
        public void TearDown()
        {
            if (combatSettings != null)
                Object.DestroyImmediate(combatSettings);
        }

        [Test]
        public void ResolveCombat_StrongerUnitShouldWin()
        {
            CombatResult result = resolver.ResolveCombat(redUnit, blueUnit);

            Assert.IsNotNull(result);
            Assert.AreEqual(redUnit, result.Winner, "Red (strength 5) should beat Blue (strength 3)");
            Assert.AreEqual(blueUnit, result.Loser);
        }

        [Test]
        public void ResolveCombat_WeakerUnitShouldLose()
        {
            var greenUnit = new MockCombatant(UnitType.Green, 100f);
            var yellowUnit = new MockCombatant(UnitType.Yellow, 100f);

            CombatResult result = resolver.ResolveCombat(greenUnit, yellowUnit);

            Assert.IsNotNull(result);
            Assert.AreEqual(greenUnit, result.Winner, "Green (strength 7) should beat Yellow (strength 1)");
            Assert.AreEqual(yellowUnit, result.Loser);
        }

        [Test]
        public void ResolveCombat_ShouldCalculateCorrectDamageToWinner()
        {
            CombatResult result = resolver.ResolveCombat(redUnit, blueUnit);

            float expectedDamage = 100f * 10f / 100f; // maxHealth * winnerDamagePercent / 100
            Assert.AreEqual(expectedDamage, result.DamageToWinner, 0.01f);
        }

        [Test]
        public void ResolveCombat_WithRandomWinChance_ShouldAllowWeakerToWin()
        {
            combatSettings.randomWinChance = 1f; // 100% chance for weaker to win

            CombatResult result = resolver.ResolveCombat(redUnit, blueUnit);

            Assert.IsNotNull(result);
            Assert.AreEqual(blueUnit, result.Winner, "Blue should win with 100% random win chance");
            Assert.AreEqual(redUnit, result.Loser);
        }

        [Test]
        public void ResolveCombat_WithZeroRandomChance_StrongerAlwaysWins()
        {
            combatSettings.randomWinChance = 0f;

            for (int i = 0; i < 10; i++)
            {
                CombatResult result = resolver.ResolveCombat(redUnit, blueUnit);
                Assert.AreEqual(redUnit, result.Winner, $"Red should always win on iteration {i}");
            }
        }

        [Test]
        public void ResolveCombat_EqualStrength_ShouldResolveConsistently()
        {
            var unit1 = new MockCombatant(UnitType.Red, 100f);
            var unit2 = new MockCombatant(UnitType.Red, 100f);

            CombatResult result = resolver.ResolveCombat(unit1, unit2);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Winner == unit1 || result.Winner == unit2);
            Assert.IsTrue(result.Loser == unit1 || result.Loser == unit2);
            Assert.AreNotEqual(result.Winner, result.Loser);
        }

        [Test]
        public void GetStrength_ShouldReturnCorrectValue()
        {
            Assert.AreEqual(5, combatSettings.GetStrength(UnitType.Red));
            Assert.AreEqual(3, combatSettings.GetStrength(UnitType.Blue));
            Assert.AreEqual(7, combatSettings.GetStrength(UnitType.Green));
            Assert.AreEqual(1, combatSettings.GetStrength(UnitType.Yellow));
        }

        [Test]
        public void GetStrength_UndefinedUnit_ShouldReturnDefaultValue()
        {
            var newSettings = ScriptableObject.CreateInstance<CombatSettings>();
            newSettings.unitStrengths = new System.Collections.Generic.List<CombatSettings.UnitStrength>();

            int strength = newSettings.GetStrength(UnitType.Red);

            Assert.AreEqual(1, strength, "Should return default strength of 1");

            Object.DestroyImmediate(newSettings);
        }

        [Test]
        public void GetStrongerUnit_ShouldReturnCorrectUnit()
        {
            Assert.AreEqual(UnitType.Red, combatSettings.GetStrongerUnit(UnitType.Red, UnitType.Blue));
            Assert.AreEqual(UnitType.Green, combatSettings.GetStrongerUnit(UnitType.Red, UnitType.Green));
            Assert.AreEqual(UnitType.Red, combatSettings.GetStrongerUnit(UnitType.Yellow, UnitType.Red));
        }

        private class MockCombatant : ICombatant
        {
            public UnitType UnitType { get; }
            public float MaxHealth { get; }
            public float CurrentHealth { get; private set; }
            public bool IsDead => CurrentHealth <= 0;
            public GameObject GameObject => null;
            public UnitController Controller => null;
            public int CombatLayer => 0;
            public float DetectionRadius => 2f;

            private bool victoryCallbackCalled;
            private bool defeatCallbackCalled;

            public MockCombatant(UnitType type, float maxHealth)
            {
                UnitType = type;
                MaxHealth = maxHealth;
                CurrentHealth = maxHealth;
            }

            public void TakeDamage(float amount)
            {
                CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            }

            public bool CanEngageCombat() => !IsDead;

            public void OnCombatVictory()
            {
                victoryCallbackCalled = true;
            }

            public void OnCombatDefeat()
            {
                defeatCallbackCalled = true;
            }

            public bool WasVictoryCalled() => victoryCallbackCalled;
            public bool WasDefeatCalled() => defeatCallbackCalled;
        }
    }
}
#endif