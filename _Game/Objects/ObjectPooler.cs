using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts._Game.Objects
{
    public class ObjectPooler : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public GameObject prefab;
            public int size = 10;
            public int maxSize = 1000;
        }

        public List<Pool> pools;
        public Dictionary<string, ObjectPool<GameObject>> poolDictionary;

        private void Start()
        {
            poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();
            foreach (var pool in pools)
            {
                var objectPool = new ObjectPool<GameObject>(
                    createFunc: () =>
                    {
                        var obj = Instantiate(pool.prefab);
                        var pooledObjects = obj.GetComponents<IPooledObject>().Cast<IPooledObject>().ToList();
                        pooledObjects.ForEach(o => o.ObjectPooler = this);
                        obj.name = pool.prefab.name;
                        return obj;
                    },
                    actionOnGet: obj => { obj.SetActive(true); },
                    actionOnRelease: obj => { obj.SetActive(false); },
                    actionOnDestroy: obj => Destroy(obj.gameObject),
                    collectionCheck: true,
                    defaultCapacity: pool.size,
                    maxSize: pool.maxSize
                    );
                poolDictionary.Add(pool.prefab.name, objectPool);
            }
        }

        public GameObject Create(GameObject gameObject)
        {
            return poolDictionary[gameObject.name].Get();
        }

        public GameObject CreatePlayerObject(GameObject gameObject, Transform playerInputTransform)
        {
            var obj = poolDictionary[gameObject.name].Get();
            obj.GetComponentsInChildren<IPlayerObject>().ToList().ForEach(o =>
            {
                o.PlayerInputTransform = playerInputTransform;
            });
            return obj;
        }

        public void Release(GameObject gameObject)
        {
            poolDictionary[gameObject.name].Release(gameObject);
        }
    }
}