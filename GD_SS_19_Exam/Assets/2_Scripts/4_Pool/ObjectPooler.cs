using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }
    
    public class ObjectPooler : MonoBehaviour
    {
        #region Singleton

        public static ObjectPooler Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
        }

        #endregion
        
        public List<Pool> pools;

        public Dictionary<GameObject, Queue<GameObject>> poolDictionary =
            new Dictionary<GameObject, Queue<GameObject>>();

        private void Start()
        {
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.prefab, objectPool);
            }
        }

        public void Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                Debug.Log("Prefab '" + prefab.name + "' is not part of the Pools! Make sure to add it to the list!");
                return;
            }
            
            GameObject objectToSpawn = poolDictionary[prefab].Dequeue();

            objectToSpawn.transform.parent = parent;
            
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

            if (pooledObject != null)
            {
                pooledObject.OnObjectSpawn();
            }

            poolDictionary[prefab].Enqueue(objectToSpawn);
        }
        
        public void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                Debug.Log("Prefab '" + prefab.name + "' is not part of the Pools! Make sure to add it to the list!");
                return;
            }
            
            GameObject objectToSpawn = poolDictionary[prefab].Dequeue();

            objectToSpawn.transform.parent = null;
            
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

            if (pooledObject != null)
            {
                pooledObject.OnObjectSpawn();
            }

            poolDictionary[prefab].Enqueue(objectToSpawn);
        }

        public void Destroy(GameObject prefab)
        {
            prefab.transform.parent = transform;
            prefab.SetActive(false);
        }
    }
}
