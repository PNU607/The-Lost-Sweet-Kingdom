using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<EnemyData, Queue<GameObject>> pool = new();

    private Dictionary<EnemyData, Transform> poolParents = new();

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
            Debug.LogError("EnemyData is null");
            return null;
        }

        if (!pool.ContainsKey(data))
        {
            pool[data] = new Queue<GameObject>();

            GameObject parentGO = new GameObject(data.name + "_Pool");
            parentGO.transform.SetParent(this.transform);
            poolParents[data] = parentGO.transform;
        }

        GameObject enemy;

        if (pool[data].Count > 0)
        {
            enemy = pool[data].Dequeue();
        }
        else
        {
            enemy = Instantiate(data.enemyPrefab);
            enemy.transform.SetParent(poolParents[data]);
        }

        Enemy enemyTest = enemy.GetComponent<Enemy>();
        if (enemyTest == null)
        {
            Debug.LogError("Enemy Test Null");
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

        Enemy enemyTest = enemy.GetComponent<Enemy>();
        if (enemyTest == null || enemyTest.GetEnemyData() == null)
        {
            Debug.LogWarning("No Data EnemyTest");
            return;
        }

        EnemyData data = enemyTest.GetEnemyData();

        if (!pool.ContainsKey(data))
        {
            pool[data] = new Queue<GameObject>();

            GameObject parentGO = new GameObject(data.name + "_Pool");
            parentGO.transform.SetParent(this.transform);
            poolParents[data] = parentGO.transform;
        }

        enemy.transform.SetParent(poolParents[data]);
        pool[data].Enqueue(enemy);
    }
}

