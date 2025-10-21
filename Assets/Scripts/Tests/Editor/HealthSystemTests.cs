#if UNITY_EDITOR
using NUnit.Framework;
using ClashingArmies.Health;

namespace ClashingArmies.Tests
{
    public class HealthSystemTests
    {
        private HealthSystem healthSystem;
        private const float MaxHealth = 100f;

        [SetUp]
        public void SetUp()
        {
            healthSystem = new HealthSystem(MaxHealth);
        }

        [Test]
        public void Constructor_ShouldInitializeWithMaxHealth()
        {
            Assert.AreEqual(MaxHealth, healthSystem.MaxHealth);
            Assert.AreEqual(MaxHealth, healthSystem.CurrentHealth);
        }

        [Test]
        public void TakeDamage_ShouldReduceHealth()
        {
            healthSystem.TakeDamage(30f);

            Assert.AreEqual(70f, healthSystem.CurrentHealth);
        }

        [Test]
        public void TakeDamage_ShouldNotGoBelowZero()
        {
            healthSystem.TakeDamage(150f);

            Assert.AreEqual(0f, healthSystem.CurrentHealth);
        }

        [Test]
        public void TakeDamage_WithNegativeValue_ShouldNotHeal()
        {
            healthSystem.TakeDamage(50f);
            healthSystem.TakeDamage(-20f);

            Assert.AreEqual(50f, healthSystem.CurrentHealth, "Negative damage should be treated as 0");
        }

        [Test]
        public void TakeDamage_ShouldInvokeHealthChangedEvent()
        {
            bool eventInvoked = false;
            float receivedCurrent = 0f;
            float receivedMax = 0f;

            healthSystem.OnHealthChanged += (current, max) =>
            {
                eventInvoked = true;
                receivedCurrent = current;
                receivedMax = max;
            };

            healthSystem.TakeDamage(25f);

            Assert.IsTrue(eventInvoked, "OnHealthChanged event should be invoked");
            Assert.AreEqual(75f, receivedCurrent);
            Assert.AreEqual(MaxHealth, receivedMax);
        }

        [Test]
        public void TakeDamage_ShouldInvokeDeathEventWhenHealthReachesZero()
        {
            bool deathEventInvoked = false;
            healthSystem.OnDeath += () => deathEventInvoked = true;

            healthSystem.TakeDamage(100f);

            Assert.IsTrue(deathEventInvoked, "OnDeath event should be invoked");
        }

        [Test]
        public void TakeDamage_WhenAlreadyDead_ShouldNotInvokeEvents()
        {
            healthSystem.TakeDamage(100f);

            int healthChangedCount = 0;
            int deathEventCount = 0;

            healthSystem.OnHealthChanged += (current, max) => healthChangedCount++;
            healthSystem.OnDeath += () => deathEventCount++;

            healthSystem.TakeDamage(50f);

            Assert.AreEqual(0, healthChangedCount, "OnHealthChanged should not be invoked when already dead");
            Assert.AreEqual(0, deathEventCount, "OnDeath should not be invoked again");
        }

        [Test]
        public void TakeDamage_MultipleHits_ShouldAccumulateDamage()
        {
            healthSystem.TakeDamage(20f);
            healthSystem.TakeDamage(30f);
            healthSystem.TakeDamage(10f);

            Assert.AreEqual(40f, healthSystem.CurrentHealth);
        }

        [Test]
        public void ResetHealth_ShouldRestoreToMaxHealth()
        {
            healthSystem.TakeDamage(60f);
            healthSystem.ResetHealth();

            Assert.AreEqual(MaxHealth, healthSystem.CurrentHealth);
        }

        [Test]
        public void ResetHealth_ShouldInvokeHealthChangedEvent()
        {
            bool eventInvoked = false;
            float receivedCurrent = 0f;

            healthSystem.TakeDamage(50f);
            
            healthSystem.OnHealthChanged += (current, max) =>
            {
                eventInvoked = true;
                receivedCurrent = current;
            };

            healthSystem.ResetHealth();

            Assert.IsTrue(eventInvoked);
            Assert.AreEqual(MaxHealth, receivedCurrent);
        }

        [Test]
        public void ResetHealth_AfterDeath_ShouldRestoreHealth()
        {
            healthSystem.TakeDamage(100f);
            Assert.AreEqual(0f, healthSystem.CurrentHealth);

            healthSystem.ResetHealth();

            Assert.AreEqual(MaxHealth, healthSystem.CurrentHealth);
        }

        [Test]
        public void TakeDamage_ExactlyMaxHealth_ShouldTriggerDeath()
        {
            bool deathEventInvoked = false;
            healthSystem.OnDeath += () => deathEventInvoked = true;

            healthSystem.TakeDamage(MaxHealth);

            Assert.AreEqual(0f, healthSystem.CurrentHealth);
            Assert.IsTrue(deathEventInvoked);
        }

        [Test]
        public void TakeDamage_SmallIncrements_ShouldWorkCorrectly()
        {
            for (int i = 0; i < 10; i++)
            {
                healthSystem.TakeDamage(5f);
            }

            Assert.AreEqual(50f, healthSystem.CurrentHealth);
        }

        [Test]
        public void HealthSystem_WithZeroMaxHealth_ShouldHandleGracefully()
        {
            var zeroHealthSystem = new HealthSystem(0f);

            Assert.AreEqual(0f, zeroHealthSystem.MaxHealth);
            Assert.AreEqual(0f, zeroHealthSystem.CurrentHealth);
        }

        [Test]
        public void OnHealthChanged_ShouldProvideCorrectValues()
        {
            int eventCount = 0;
            float lastCurrent = 0f;
            float lastMax = 0f;

            healthSystem.OnHealthChanged += (current, max) =>
            {
                eventCount++;
                lastCurrent = current;
                lastMax = max;
            };

            healthSystem.TakeDamage(25f);
            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(75f, lastCurrent);
            Assert.AreEqual(MaxHealth, lastMax);

            healthSystem.TakeDamage(25f);
            Assert.AreEqual(2, eventCount);
            Assert.AreEqual(50f, lastCurrent);
            Assert.AreEqual(MaxHealth, lastMax);
        }
    }
}
#endif