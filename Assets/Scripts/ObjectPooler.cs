using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 5;
    private List<GameObject> pool;

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public GameObject getPooledObject()
    {
        foreach(GameObject obj in pool)
        {
            if(!obj.activeSelf) return obj;
        }
        return CreateNewObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
