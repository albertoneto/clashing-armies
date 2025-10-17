using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies
{
    public class PoolingSystem : MonoBehaviour
    {
        public enum PoolType {Unit}
        [System.Serializable]
        public class Pool
        {
            public PoolType poolType;
            public GameObject prefab;
            public int size;
        }
        [SerializeField] private List<Pool> pools = new List<Pool>();

        private readonly Dictionary<PoolType, Queue<GameObject>> _poolDictionary = new ();
        
        public void Awake()
        {
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform, true);
                    objectPool.Enqueue(obj);
                    obj.SetActive(false);
                }

                _poolDictionary.Add(pool.poolType, objectPool);
            }
        }

        public GameObject SpawnFromPool(PoolType poolType, Vector3 position, Transform parent)
        {
            if (!_poolDictionary.ContainsKey(poolType))
            {
                Debug.LogWarning("Pool with type " + poolType + " doesn't exist.");
                return null;
            }
            if (_poolDictionary[poolType].Count == 0)
            {
                Debug.LogError("Empty pool: " + poolType);
                return null;
            }

            GameObject objectToSpawn = _poolDictionary[poolType].Dequeue();

            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.position = position;
            var pos = objectToSpawn.transform.localPosition;
            objectToSpawn.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        public void ReturnToPool(PoolType poolTypes, GameObject objectToReturn)
        {
            if (!_poolDictionary.ContainsKey(poolTypes))
            {
                Debug.LogWarning("Pool with type " + poolTypes + " doesn't exist.");
                return;
            }

            if (objectToReturn == null)
            {
                Debug.LogWarning($"Null GameObject of {poolTypes} returning.");
                return;
            }

            objectToReturn.SetActive(false);
            objectToReturn.transform.parent = transform;
            _poolDictionary[poolTypes].Enqueue(objectToReturn);
        }
    }
}