using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnDelay = 1f;
    public float startDelay = 1f;
    public GameObject[] enemyTypes;
    public Transform spawnPoint;

    private bool isGameRunning = false;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    public void OnStartButtonPressed() 
    {
        isGameRunning = true;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(startDelay);

        while (isGameRunning)
        {
            GameObject enemyPrefab = enemyTypes[Random.Range(0, enemyTypes.Length)];

            GameObject enemy = ObjectPool.Instance.GetEnemy(enemyPrefab);

            enemy.transform.position = spawnPoint.position;

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
