using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public List<WaveData> waves;
    public Transform spawnPoint;

    public int currentWaveIndex = 0;
    public bool isGameRunning = false;

    public TextMeshProUGUI waveText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        UpdateWaveText();
    }

    public void StartGame()
    {
        if (!isGameRunning)
        {
            isGameRunning = true;
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("더 이상 웨이브 없음");
            yield break;
        }

        WaveData wave = waves[currentWaveIndex];
        UpdateWaveText();

        yield return new WaitForSeconds(wave.startDelay);
        Debug.Log($"Start Wave : {currentWaveIndex + 1}");

        foreach (var enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                GameObject enemy = ObjectPool.Instance.GetEnemy(enemyInfo.enemyData);
                enemy.transform.position = spawnPoint.position;

                yield return new WaitForSeconds(enemyInfo.spawnDelay);
            }
        }

        Debug.Log("Spawn 종료");
    }

    public void UpdateWaveText()
    {
        waveText.text = $"Wave : {currentWaveIndex + 1}";
    }
}
