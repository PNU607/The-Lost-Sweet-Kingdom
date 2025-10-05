using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static WaveData;

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

        List<EnemySpawnInfo> fullEnemyList = new List<EnemySpawnInfo>();
        foreach (var enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                fullEnemyList.Add(enemyInfo);
            }
        }

        List<List<EnemySpawnInfo>> enemyGroups = new List<List<EnemySpawnInfo>>();
        List<EnemySpawnInfo> currentGroup = new List<EnemySpawnInfo>();
        List<EnemySpawnInfo> finalSpawnList = new List<EnemySpawnInfo>();

        foreach (var enemyInfo in fullEnemyList)
        {
            if (enemyInfo.enemyData.name.Contains("Lollipop"))
            {
                if (currentGroup.Count > 0)
                {
                    Shuffle(currentGroup);
                    finalSpawnList.AddRange(currentGroup);
                }

                finalSpawnList.Add(enemyInfo);
                currentGroup = new List<EnemySpawnInfo>();
            }
            else
            {
                currentGroup.Add(enemyInfo);
            }
        }

        if (currentGroup.Count > 0)
        {
            Shuffle(currentGroup);
            finalSpawnList.AddRange(currentGroup);
        }

        foreach (var enemyInfo in finalSpawnList)
        {
            GameObject enemy = ObjectPool.Instance.GetEnemy(enemyInfo.enemyData);
            enemy.transform.position = spawnPoint.position;

            yield return new WaitForSeconds(enemyInfo.spawnDelay);
        }

        Debug.Log("Spawn 종료");
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void UpdateWaveText()
    {
        waveText.text = $"Wave : {currentWaveIndex + 1}";
    }
}
