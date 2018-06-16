using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
	// Straight copy from a Brackeys video with added colors to make it more fun
	// Understand where to use it, but not my code

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        float redValue = Random.Range(0, 2), greenValue = Random.Range(0, 2), blueValue = Random.Range(0, 2);

        Renderer rend = objectToSpawn.GetComponent<Renderer>();
        rend.material.SetColor("_Color", new Color(redValue, greenValue, blueValue));
        
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
