using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool : MonoBehaviour
{
    public int m_BaseNumToSpawn;

    private GameObject m_MasterPrefab;        //Prefab used to spawn the pool
    public List<GameObject> m_PooledObjects = new List<GameObject>();

    public void Setup(GameObject objPrefab, int numToSpawn = 10)
    {
        //Set Master Prefab used for pool
        m_MasterPrefab = objPrefab;
        m_BaseNumToSpawn = numToSpawn;

        //Spawn Initial Pool
        for (int i = 0; i < m_BaseNumToSpawn; ++i)
        {
            GameObject newObj = CreateNewObject();
        }
    }

    public void Reset()
    {
        foreach (GameObject currObj in m_PooledObjects)
        {
            currObj.SetActive(false);
        }
    }

    //Returns an inactive gameobject from pool
    public GameObject GetObjectFromPool()
    {
        foreach (GameObject currObj in m_PooledObjects)
        {
            if (!currObj.activeInHierarchy)
            {
                return currObj;
            }
        }
        return CreateNewObject();
    }

    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(m_MasterPrefab, transform);
        m_PooledObjects.Add(newObj);
        newObj.SetActive(false);
        return newObj;
    }
}
