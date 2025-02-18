using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<WaveData> waves;
    public Transform spawnPoint;

    private int currentWaveIndex = 0;
    private bool isGameRunning = false;

    public void StartGame()
    {
        if (isGameRunning == false)
        {
            isGameRunning = true;
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        WaveData wave = waves[currentWaveIndex];

        yield return new WaitForSeconds(wave.startDelay);

        Debug.Log($"Start Wave : {currentWaveIndex}");

        foreach (var enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                GameObject enemy = ObjectPool.Instance.GetEnemy(enemyInfo.enemyPrefab);
                enemy.transform.position = spawnPoint.position;
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
            }
        }

        currentWaveIndex++;
        yield return new WaitForSeconds(3f);
        
        isGameRunning = false;
    }
}
