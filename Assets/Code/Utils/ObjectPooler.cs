using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    Dictionary<string, Queue<GameObject>> poolDictionary;


    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // ahora recorremos la lista de pools para crear y añadir las Queues
        foreach (Pool pool in pools)
        {
            // y creamos la Queue
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // despues creamos cada uno de los Gameobjects de la Pool
            // los desactivamos y los añadimos a la Queue
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            // por ultimo añadimos el pool al diccionario
            poolDictionary.Add(pool.tag, objectPool);
        }
    }


    public void SpawnFromPool (string tag)
    {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();        
        objectToSpawn.SetActive(true);        

        // volvemos a meter el objeto al final de la Cola
        poolDictionary[tag].Enqueue(objectToSpawn);        
    }
    
}
