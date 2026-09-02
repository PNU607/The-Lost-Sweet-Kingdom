using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private readonly Dictionary<string, Queue<GameObject>> pool = new();

    private readonly Dictionary<string, Transform> poolParents = new();

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

        string enemyId = data.enemyId;
        if (!pool.ContainsKey(enemyId))
        {
            pool[enemyId] = new Queue<GameObject>();

            GameObject parentGO = new GameObject(enemyId + "_Pool");
            parentGO.transform.SetParent(this.transform);
            poolParents[enemyId] = parentGO.transform;
        }

        GameObject enemy;

        if (pool[enemyId].Count > 0)
        {
            enemy = pool[enemyId].Dequeue();
        }
        else
        {
            GameObject prefab = data.enemyPrefab;
            if (prefab == null)
            {
                Debug.LogError($"Enemy prefab not found: {data.enemyPrefabAssetName}");
                return null;
            }
            enemy = Instantiate(prefab);
            enemy.transform.SetParent(poolParents[enemyId]);
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

        string enemyId = data.enemyId;
        if (!pool.ContainsKey(enemyId))
        {
            pool[enemyId] = new Queue<GameObject>();

            GameObject parentGO = new GameObject(enemyId + "_Pool");
            parentGO.transform.SetParent(this.transform);
            poolParents[enemyId] = parentGO.transform;
        }

        enemy.transform.SetParent(poolParents[enemyId]);
        pool[enemyId].Enqueue(enemy);
    }
}

