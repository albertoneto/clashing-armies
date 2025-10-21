using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies
{
    public class PoolingSystem : MonoBehaviour
    {
        public enum PoolType {Unit, ParticleExplosion, ParticleSpawn, ParticleVictory, AudioSource}
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
                    var obj = InstantiateObject(pool);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.poolType, objectPool);
            }
        }

        private GameObject InstantiateObject(Pool pool)
        {
            GameObject obj = Instantiate(pool.prefab, transform, true);
            obj.SetActive(false);
            return obj;
        }

        public GameObject SpawnFromPool(PoolType poolType, Vector3 position, Transform parent, string name = null)
        {
            if (!_poolDictionary.ContainsKey(poolType))
            {
                Debug.LogWarning("Pool with type " + poolType + " doesn't exist.");
                return null;
            }

            if (_poolDictionary[poolType].Count == 0)
            {
                Debug.LogWarning("Empty pool: " + poolType);Pool pool = pools.Find(p => p.poolType == poolType);
                
                GameObject instantiateObject = InstantiateObject(pool);
                SetObject(position, parent, name, instantiateObject);
                return instantiateObject;
            }

            GameObject objectToSpawn = _poolDictionary[poolType].Dequeue();
            SetObject(position, parent, name, objectToSpawn);

            return objectToSpawn;
        }

        private static void SetObject(Vector3 position, Transform parent, string name, GameObject objectToSpawn)
        {
            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.position = position;
            if(!string.IsNullOrEmpty(name)) objectToSpawn.name = name;
            objectToSpawn.SetActive(true);
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

            objectToReturn.name = "pool object";
            objectToReturn.SetActive(false);
            objectToReturn.transform.parent = transform;
            _poolDictionary[poolTypes].Enqueue(objectToReturn);
        }
    }
}