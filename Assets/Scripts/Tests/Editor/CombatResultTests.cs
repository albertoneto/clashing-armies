#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using ClashingArmies.Combat;
using ClashingArmies.Units;

namespace ClashingArmies.Tests
{
    public class CombatResultTests
    {
        private MockCombatant winner;
        private MockCombatant loser;
        private CombatResult combatResult;

        [SetUp]
        public void SetUp()
        {
            winner = new MockCombatant(UnitType.Red, 100f);
            loser = new MockCombatant(UnitType.Blue, 100f);
            combatResult = new CombatResult(winner, loser, 10f);
        }

        [Test]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            Assert.AreEqual(winner, combatResult.Winner);
            Assert.AreEqual(loser, combatResult.Loser);
            Assert.AreEqual(10f, combatResult.DamageToWinner);
        }

        [Test]
        public void Apply_ShouldDamageWinner()
        {
            float initialHealth = winner.CurrentHealth;
            
            combatResult.Apply();

            Assert.AreEqual(initialHealth - 10f, winner.CurrentHealth);
        }

        [Test]
        public void Apply_ShouldKillLoser()
        {
            combatResult.Apply();

            Assert.AreEqual(0f, loser.CurrentHealth);
            Assert.IsTrue(loser.IsDead);
        }

        [Test]
        public void Apply_ShouldCallWinnerOnCombatVictory()
        {
            combatResult.Apply();

            Assert.IsTrue(winner.VictoryCalled);
        }

        [Test]
        public void Apply_ShouldCallLoserOnCombatDefeat()
        {
            combatResult.Apply();

            Assert.IsTrue(loser.DefeatCalled);
        }

        [Test]
        public void Apply_ShouldApplyInCorrectOrder()
        {
            combatResult.Apply();

            Assert.AreEqual(90f, winner.CurrentHealth, "Winner should have taken damage");
            Assert.AreEqual(0f, loser.CurrentHealth, "Loser should be dead");
            Assert.IsTrue(winner.VictoryCalled, "Winner callback should be called");
            Assert.IsTrue(loser.DefeatCalled, "Loser callback should be called");
        }

        [Test]
        public void Apply_WithZeroDamage_ShouldNotDamageWinner()
        {
            var result = new CombatResult(winner, loser, 0f);
            
            result.Apply();

            Assert.AreEqual(100f, winner.CurrentHealth);
        }

        [Test]
        public void Apply_WithHighDamage_ShouldNotKillWinner()
        {
            var result = new CombatResult(winner, loser, 50f);
            
            result.Apply();

            Assert.AreEqual(50f, winner.CurrentHealth);
            Assert.IsFalse(winner.IsDead, "Winner should survive");
        }

        [Test]
        public void Apply_LoserShouldReceiveMaxHealthDamage()
        {
            combatResult.Apply();

            Assert.AreEqual(0f, loser.CurrentHealth);
        }

        [Test]
        public void CombatResult_WithPartialHealthUnits_ShouldWorkCorrectly()
        {
            winner.TakeDamage(30f);
            loser.TakeDamage(20f);

            var result = new CombatResult(winner, loser, 5f);
            result.Apply();

            Assert.AreEqual(65f, winner.CurrentHealth);
            Assert.AreEqual(0f, loser.CurrentHealth);
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

            public bool VictoryCalled { get; private set; }
            public bool DefeatCalled { get; private set; }

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
                VictoryCalled = true;
            }

            public void OnCombatDefeat()
            {
                DefeatCalled = true;
            }
        }
    }
}
#endif