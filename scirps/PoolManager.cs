using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public GameObject[] objectsToPool;  // ����Ԥ��������
    public int poolSize = 10;          // ��ʼ�ش�С

    private Dictionary<string, Queue<GameObject>> poolDictionary; // �ص��ֵ�

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // ��ʼ�������
        foreach (GameObject prefab in objectsToPool)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.name = prefab.name; // ȷ������һ��
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
            obj.SetActive(true); // ��������
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

        obj.SetActive(false); // ��������
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); // ����λ�ú���ת
        poolDictionary[prefabName].Enqueue(obj);
    }
}
