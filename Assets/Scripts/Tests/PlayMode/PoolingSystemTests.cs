using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ClashingArmies.Tests
{
    public class PoolingSystemTests : PoolingSystemTestBase
    {
        [SetUp]
        public void SetUp()
        {
            SetupPoolingSystem(5);
        }

        [TearDown]
        public void TearDown()
        {
            TearDownPoolingSystem();
        }

        [Test]
        public void Awake_ShouldCreatePoolWithCorrectSize()
        {
            var poolDictionaryField = typeof(PoolingSystem).GetField("_poolDictionary", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var poolDictionary = poolDictionaryField.GetValue(poolingSystem) as System.Collections.Generic.Dictionary<PoolingSystem.PoolType, System.Collections.Generic.Queue<GameObject>>;

            Assert.IsNotNull(poolDictionary);
            Assert.IsTrue(poolDictionary.ContainsKey(PoolingSystem.PoolType.Unit));
            Assert.AreEqual(5, poolDictionary[PoolingSystem.PoolType.Unit].Count);
        }

        [Test]
        public void SpawnFromPool_ShouldReturnObject()
        {
            Vector3 position = new Vector3(1, 2, 3);
            GameObject spawnedObject = poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, position, null);

            Assert.IsNotNull(spawnedObject);
            Assert.IsTrue(spawnedObject.activeSelf);
            Assert.AreEqual(position, spawnedObject.transform.position);
        }

        [Test]
        public void SpawnFromPool_MultipleSpawns_ShouldReducePoolSize()
        {
            Vector3 position = Vector3.zero;

            poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, position, null);
            poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, position, null);

            var poolDictionaryField = typeof(PoolingSystem).GetField("_poolDictionary", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var poolDictionary = poolDictionaryField.GetValue(poolingSystem) as System.Collections.Generic.Dictionary<PoolingSystem.PoolType, System.Collections.Generic.Queue<GameObject>>;

            Assert.AreEqual(3, poolDictionary[PoolingSystem.PoolType.Unit].Count);
        }

        [Test]
        public void SpawnFromPool_WithInvalidPoolType_ShouldReturnNull()
        {
            PoolingSystem.PoolType invalidType = (PoolingSystem.PoolType)999;

            LogAssert.Expect(LogType.Warning, "Pool with type " + invalidType + " doesn't exist.");
            GameObject result = poolingSystem.SpawnFromPool(invalidType, Vector3.zero, null);

            Assert.IsNull(result);
        }

        [Test]
        public void SpawnFromPool_EmptyPool_ShouldReturnNull()
        {
            for (int i = 0; i < 5; i++)
            {
                poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, Vector3.zero, null);
            }

            LogAssert.Expect(LogType.Error, "Empty pool: " + PoolingSystem.PoolType.Unit);
            GameObject result = poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, Vector3.zero, null);

            Assert.IsNull(result);
        }

        [Test]
        public void ReturnToPool_ShouldAddObjectBackToPool()
        {
            GameObject spawnedObject = poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, Vector3.zero, null);
            poolingSystem.ReturnToPool(PoolingSystem.PoolType.Unit, spawnedObject);

            var poolDictionaryField = typeof(PoolingSystem).GetField("_poolDictionary", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var poolDictionary = poolDictionaryField.GetValue(poolingSystem) as System.Collections.Generic.Dictionary<PoolingSystem.PoolType, System.Collections.Generic.Queue<GameObject>>;

            Assert.AreEqual(5, poolDictionary[PoolingSystem.PoolType.Unit].Count);
            Assert.IsFalse(spawnedObject.activeSelf);
        }

        [Test]
        public void ReturnToPool_WithNullObject_ShouldLogWarning()
        {
            LogAssert.Expect(LogType.Warning, $"Null GameObject of {PoolingSystem.PoolType.Unit} returning.");
            poolingSystem.ReturnToPool(PoolingSystem.PoolType.Unit, null);
        }

        [Test]
        public void ReturnToPool_WithInvalidPoolType_ShouldLogWarning()
        {
            PoolingSystem.PoolType invalidType = (PoolingSystem.PoolType)999;
            GameObject obj = new GameObject("Test");

            LogAssert.Expect(LogType.Warning, "Pool with type " + invalidType + " doesn't exist.");
            poolingSystem.ReturnToPool(invalidType, obj);

            Object.DestroyImmediate(obj);
        }

        [Test]
        public void SpawnFromPool_WithParent_ShouldSetParentCorrectly()
        {
            GameObject parent = new GameObject("Parent");
            Vector3 position = Vector3.zero;

            GameObject spawnedObject = poolingSystem.SpawnFromPool(PoolingSystem.PoolType.Unit, position, parent.transform);

            Assert.IsNotNull(spawnedObject);
            Assert.AreEqual(parent.transform, spawnedObject.transform.parent);

            Object.DestroyImmediate(parent);
        }
    }
}