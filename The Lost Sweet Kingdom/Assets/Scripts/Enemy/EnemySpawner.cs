using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public List<WaveData> waves;
    public Transform spawnPoint;

    private int currentWaveIndex = 0;
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
        while (currentWaveIndex < waves.Count)
        {
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

            currentWaveIndex++;
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("모든 웨이브 완료!");
        isGameRunning = false;
    }

    public void UpdateWaveText()
    {
        waveText.text = $"Wave : {currentWaveIndex + 1}";
    }
}
