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
        {
            Destroy(gameObject);
            return;
        }
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
        }
        else
        {
            enemy = Instantiate(data.enemyPrefab);
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

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        enemy.SetActive(false);

        EnemyTest enemyTest = enemy.GetComponent<EnemyTest>();
        if (enemyTest == null || enemyTest.GetEnemyData() == null)
        {
            Debug.LogWarning("Returned enemy has no valid data");
            return;
        }

        EnemyData data = enemyTest.GetEnemyData();

        if (!pool.ContainsKey(data))
        {
            pool[data] = new Queue<GameObject>();
        }

        pool[data].Enqueue(enemy);
    }
}
