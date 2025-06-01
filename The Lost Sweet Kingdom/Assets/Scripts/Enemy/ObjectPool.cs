using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<EnemyData, Queue<GameObject>> pool = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public GameObject GetEnemy(EnemyData data)
    {
        if (!pool.ContainsKey(data))
        {
            pool[data] = new Queue<GameObject>();
        }

        if (pool[data].Count > 0)
        {
            GameObject enemy = pool[data].Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            GameObject newEnemy = Instantiate(data.enemyPrefab);
            return newEnemy;
        }
    }

    public void ReturnEnemy(GameObject enemy, EnemyData data)
    {
        enemy.SetActive(false);
        if (!pool.ContainsKey(data))
        {
            pool[data] = new Queue<GameObject>();
        }
        pool[data].Enqueue(enemy);
    }
}
