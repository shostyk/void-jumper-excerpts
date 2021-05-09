using System.Collections.Generic;
using UnityEngine;
using OlegShostyk;

public class ObjectsPooler : Singleton<ObjectsPooler>
{
    #region Fields
    [SerializeField] List<Pool> pools;
    private Dictionary<MyTag, Queue<GameObject>> poolsMap;
    private Dictionary<MyTag, Pool> typeMap;
    #endregion

    #region Class Pool
    [System.Serializable]
    public class Pool
    {
        public MyTag tag;
        public GameObject prefab;
        public int initialCapacity;
        [HideInInspector] public GameObject parent;
    }
    #endregion

    void Start()
    {
        BuildPools();
    }

    private void BuildPools()
    {
        poolsMap = new Dictionary<MyTag, Queue<GameObject>>();
        typeMap = new Dictionary<MyTag, Pool>();

        foreach (Pool pool in pools)
        {
            // Creating pool parent.
            GameObject objectsParent = new GameObject("POOL " + pool.tag.ToString());
            pool.parent = objectsParent;

            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.initialCapacity; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = objectsParent.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolsMap.Add(pool.tag, objectPool);
            typeMap.Add(pool.tag, pool);
        }
    }

    public GameObject Obtain(MyTag tag)
    {
        Queue<GameObject> objectPool;
        poolsMap.TryGetValue(tag, out objectPool);

        if (objectPool == null)
        {
            Debug.LogError("[ObjectPooler::Obtain] there is no objectPool with tag = " + tag);
            return null;
        }
        else
        {
            // Return inactive gameobject from the pool.
            if (objectPool.Count > 0)
                return objectPool.Dequeue();

            // Else create new gameobject of that type.
            Pool pool = typeMap[tag];
            GameObject obj = Instantiate(pool.prefab);
            obj.transform.parent = pool.parent.transform;
            obj.SetActive(false);

            return obj;
        }
    }

    public void Free(MyTag tag, GameObject obj)
    {
        // Deactivate in any way.
        obj.SetActive(false);

        Queue<GameObject> objectPool;
        poolsMap.TryGetValue(tag, out objectPool);

        if (objectPool == null)
        {
            Debug.LogError("[ObjectPooler::Free] there is no objectPool with tag = " + tag);
        }
        else
        {
            objectPool.Enqueue(obj);
        }
    }
}
