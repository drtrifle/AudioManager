using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    //Note: Ensure that the order that eums appear here is same as the order in the m_prefabList
    //public List<ObjectPoolEnum> m_EnumList;
    //public List<GameObject> m_PrefabList;

    public List<GameObject> m_PoolPrefabList;

    //Used to map request to the correct object pool 
    Dictionary<GameObject, ObjectPool> m_ObjPoolDict;
    
    #region PoolManager Methods
    void SetUpAllPools()
    {
        //Create Dictionary
        m_ObjPoolDict = new Dictionary<GameObject, ObjectPool>();

        //Loop through each lists, create pools and populate dictionary
        for (int i = 0 ; i < m_PoolPrefabList.Count; ++i)
        {
            CreateObjectPool(m_PoolPrefabList[i]);
        }
    }

    void CreateObjectPool(GameObject prefabRef)
    {
        GameObject poolFolder = new GameObject(prefabRef.name + " Pool");
        poolFolder.transform.parent = transform;

        ObjectPool currPool = poolFolder.AddComponent<ObjectPool>();

        currPool.Setup(prefabRef);

        //Add to Dict
        m_ObjPoolDict.Add(prefabRef, currPool);

        Debug.Log("Created new ObjectPool for " + prefabRef.name);
    }

    void ResetAllPools()
    {
        List<ObjectPool> poolList = m_ObjPoolDict.Values.ToList();
        foreach (ObjectPool currPool in poolList)
        {
            currPool.Reset();
        }
    }

    public GameObject GetFromPool(GameObject poolPrefabRef)
    {
        if (poolPrefabRef == null)
        {
            Debug.LogError("Someone is trying to get null object from pool");
            return new GameObject();
        }

        if (!m_ObjPoolDict.ContainsKey(poolPrefabRef))
        {
            //Create a new Object Pool & Return an Object from it
            CreateObjectPool(poolPrefabRef);
            Debug.Log("No existing ObjectPool for " + poolPrefabRef.name + ", creating it now");

            return m_ObjPoolDict[poolPrefabRef].GetObjectFromPool();
        }
        else
        {
            return m_ObjPoolDict[poolPrefabRef].GetObjectFromPool();
        }
    }
    #endregion
}
