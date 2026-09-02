using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static WaveData;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    [SerializeField]
    private string stageId;

    [NonSerialized]
    public List<WaveData> waves = new();
    public Transform spawnPoint;

    public int currentWaveIndex = 0;
    public bool isGameRunning = false;
    public bool autoGameStart = false;

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
        LoadWavesFromExcel();
        UpdateWaveText();
    }

    private void LoadWavesFromExcel()
    {
        GameDataRepository.EnsureLoaded();

        if (string.IsNullOrWhiteSpace(stageId))
        {
            Match match = Regex.Match(
                SceneManager.GetActiveScene().name,
                @"Stage\s*(\d+)",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                stageId = $"Stage{match.Groups[1].Value}";
            }
        }

        if (string.IsNullOrWhiteSpace(stageId))
        {
            throw new InvalidOperationException(
                "EnemySpawner의 stageId를 확인할 수 없습니다. 씬 이름을 Stage1 형식으로 지정하거나 stageId를 입력하세요.");
        }

        waves = GameDataRepository.GetWavesForStage(stageId);
    }

    public void StartGame()
    {
        if (!isGameRunning)
        {
            isGameRunning = true;
            StartCoroutine(SpawnWaves());
        }
    }

    public void autoGame()
    {
        StartCoroutine(SpawnWaves());
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

        List<EnemySpawnInfo> currentGroup = new List<EnemySpawnInfo>();
        List<EnemySpawnInfo> finalSpawnList = new List<EnemySpawnInfo>();

        foreach (var enemyInfo in fullEnemyList)
        {
            if (enemyInfo.enemyData.isBoss)
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
            if (enemy == null)
            {
                Debug.LogError($"적 생성 실패: {enemyInfo.enemyId}");
                continue;
            }
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
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void UpdateWaveText()
    {
        waveText.text = $"Wave : {currentWaveIndex + 1}";
    }
}
