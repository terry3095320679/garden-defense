using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int startingSize = 12;

    private readonly List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError("SimplePool needs a Bullet prefab assigned in its Prefab field.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < startingSize; i++)
        {
            CreatePooledObject(false);
        }
    }

    public GameObject GetFromPool()
    {
        foreach (GameObject pooledObject in pooledObjects)
        {
            if (!pooledObject.activeInHierarchy)
            {
                pooledObject.SetActive(true);
                return pooledObject;
            }
        }

        return CreatePooledObject(true);
    }

    public void ReturnToPool(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }

    private GameObject CreatePooledObject(bool active)
    {
        GameObject pooledObject = Instantiate(prefab, transform);
        pooledObject.name = $"{prefab.name}_{pooledObjects.Count + 1}";
        pooledObject.SetActive(active);
        pooledObjects.Add(pooledObject);
        return pooledObject;
    }
}
