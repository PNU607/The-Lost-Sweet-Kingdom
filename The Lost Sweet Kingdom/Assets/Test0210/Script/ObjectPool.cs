using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public GameObject[] enemyPrefabs;
    public int poolSize = 10;
    private Dictionary<GameObject, Queue<GameObject>> pools;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pools = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var prefab in enemyPrefabs)
        {
            Queue<GameObject> enemyQueue = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject enemy = Instantiate(prefab);
                enemy.SetActive(false);
                enemyQueue.Enqueue(enemy);
            }
            pools.Add(prefab, enemyQueue);
        }
    }

    public GameObject GetEnemy(GameObject enemyPrefab)
    {
        if (pools.ContainsKey(enemyPrefab) && pools[enemyPrefab].Count > 0)
        {
            GameObject enemy = pools[enemyPrefab].Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            Debug.LogWarning("No more enemies in the pool. Consider increasing pool size.");
            return null;
        }
    }

    public void ReturnEnemy(GameObject enemy, GameObject enemyPrefab)
    {
        enemy.SetActive(false);
        pools[enemyPrefab].Enqueue(enemy);
    }
}
