using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public List<WaveData> waves;
    public Transform spawnPoint;

    private int currentWaveIndex = 0;
    private bool isGameRunning = false;

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
        if (isGameRunning == false)
        {
            isGameRunning = true;
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        WaveData wave = waves[currentWaveIndex];

        UpdateWaveText();

        yield return new WaitForSeconds(wave.startDelay);

        Debug.Log($"Start Wave : {currentWaveIndex + 1}");

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

    public void UpdateWaveText()
    {
        waveText.text = $"Wave : {currentWaveIndex + 1}";
    }
}
