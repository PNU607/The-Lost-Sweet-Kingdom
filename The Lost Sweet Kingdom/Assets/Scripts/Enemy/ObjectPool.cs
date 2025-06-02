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
        else
            Destroy(gameObject);
    }

    public GameObject GetEnemy(EnemyData data)
    {
        if (data == null)
        {
            Debug.LogError("EnemyData is null in ObjectPool.GetEnemy");
            return null;
        }

        if (!pool.ContainsKey(data))
        {
            pool[data] = new Queue<GameObject>();
        }

        GameObject enemy;

        if (pool[data].Count > 0)
        {
            enemy = pool[data].Dequeue();
            enemy.SetActive(true);
        }
        else
        {
            enemy = Instantiate(data.enemyPrefab);
            enemy.SetActive(true);
        }

        EnemyTest enemyTest = enemy.GetComponent<EnemyTest>();
        if (enemyTest == null)
        {
            Debug.LogError("Enemy prefab does not have EnemyTest component");
        }
        else
        {
            enemyTest.SetEnemyData(data);
        }

        return enemy;
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
