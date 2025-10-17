using UnityEngine;

namespace ClashingArmies.Tests
{
    public abstract class PoolingSystemTestBase
    {
        private GameObject poolingSystemObject;
        private GameObject unitPrefab;
        protected PoolingSystem poolingSystem;

        protected void SetupPoolingSystem(int poolSize = 10)
        {
            unitPrefab = new GameObject("UnitPrefab");
            poolingSystemObject = new GameObject("PoolingSystem");
            poolingSystem = poolingSystemObject.AddComponent<PoolingSystem>();
        
            var poolsField = typeof(PoolingSystem).GetField("pools",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
            var poolsList = new System.Collections.Generic.List<PoolingSystem.Pool>
            {
                new PoolingSystem.Pool
                {
                    poolType = PoolingSystem.PoolType.Unit,
                    prefab = unitPrefab,
                    size = poolSize
                }
            };
        
            poolsField.SetValue(poolingSystem, poolsList);
            poolingSystem.Awake();
        }

        protected void TearDownPoolingSystem()
        {
            if (poolingSystemObject != null) Object.DestroyImmediate(poolingSystemObject);
            if (unitPrefab != null) Object.DestroyImmediate(unitPrefab);
        }
    }
}
