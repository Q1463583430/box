using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public GameObject[] objectsToPool;  // 可用预览体数组
    public int poolSize = 10;          // 初始池大小

    private Dictionary<string, Queue<GameObject>> poolDictionary; // 池的字典

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // 初始化对象池
        foreach (GameObject prefab in objectsToPool)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.name = prefab.name; // 确保名字一致
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(prefab.name, objectQueue);
        }
    }

   
    public GameObject GetObjectFromPool(string prefabName)
    {
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning($"PoolManager: No pool found for {prefabName}");
            return null;
        }

        if (poolDictionary[prefabName].Count > 0)
        {
            GameObject obj = poolDictionary[prefabName].Dequeue();
            obj.SetActive(true); // 启用物体
            return obj;
        }
        else
        {
            Debug.LogWarning($"PoolManager: Pool for {prefabName} is empty");
            return null;
        }
    }

    
    public void ReturnObjectToPool(GameObject obj, string prefabName)
    {
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning($"PoolManager: No pool found for {prefabName}, destroying object.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false); // 禁用物体
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); // 重置位置和旋转
        poolDictionary[prefabName].Enqueue(obj);
    }
}
