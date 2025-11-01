using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    private string saveFolder = "Assets/Resources/ScriptableObject/WaveDatas";
    [SerializeField]
    private string stageName = "Stage1";
    [SerializeField]
    private int waveCount = 30;
    [SerializeField]
    private float totalPlayTime = 25;

    [SerializeField]
    private List<EnemyData> enemyPool;
    [SerializeField]
    private EnemyData bossData;

    public void Awake()
    {
        GenerateWaveData();
    }

    [ContextMenu("Generate Waves")]
    public void GenerateWaveData()
    {
        EnemyData enemyA = enemyPool[0];
        EnemyData enemyB = enemyPool[1];
        EnemyData enemyC = enemyPool[2];
        EnemyData enemyD = enemyPool[3];

        for (int wave = 1; wave <= waveCount; wave++)
        {
            var waveData = ScriptableObject.CreateInstance<WaveData>();
            waveData.enemies = new List<WaveData.EnemySpawnInfo>();

            // 기본 난이도 증가 곡선 (0.8배 ~ 1.5배 정도 랜덤 가중)
            float difficultyScale = 1f + (wave - 1) * 0.08f;
            float randomFactor = Random.Range(0.85f, 1.2f);
            float finalScale = difficultyScale * randomFactor;

            // Wave 10마다 보스
            if (wave % 10 == 0)
            {
                waveData.enemies.Add(new WaveData.EnemySpawnInfo
                {
                    enemyData = bossData,
                    count = Mathf.RoundToInt(1 + (wave / 10) * 0.2f),
                    spawnDelay = 1.5f
                });
            }
            else
            {
                // 패턴 선택 (랜덤)
                PatternType pattern = (PatternType)Random.Range(0, (int)PatternType.Count);

                // 패턴별 스폰 구성
                switch (pattern)
                {
                    case PatternType.Swarm:
                        AddEnemies(waveData, enemyA, Mathf.RoundToInt(5 * finalScale), 0.4f);
                        AddEnemies(waveData, enemyB, Mathf.RoundToInt(3 * finalScale), 0.5f);
                        break;

                    case PatternType.Mixed:
                        AddEnemies(waveData, enemyA, Mathf.RoundToInt(2 * finalScale), 0.6f);
                        AddEnemies(waveData, enemyC, Mathf.RoundToInt(2 * finalScale), 0.8f);
                        AddEnemies(waveData, enemyD, Mathf.RoundToInt(1 * finalScale), 0.8f);
                        break;

                    case PatternType.EliteGuard:
                        AddEnemies(waveData, enemyC, Mathf.RoundToInt(3 * finalScale), 1.2f);
                        AddEnemies(waveData, enemyD, Mathf.RoundToInt(2 * finalScale), 1.4f);
                        break;

                    case PatternType.Rush:
                        AddEnemies(waveData, enemyA, Mathf.RoundToInt(4 * finalScale), 0.25f);
                        AddEnemies(waveData, enemyB, Mathf.RoundToInt(4 * finalScale), 0.25f);
                        break;

                    case PatternType.Alternating:
                        for (int i = 0; i < Mathf.RoundToInt(4 * finalScale); i++)
                        {
                            var type = (i % 2 == 0) ? enemyB : enemyC;
                            AddEnemies(waveData, type, 1, 0.5f);
                        }
                        break;

                    case PatternType.Trickle:
                        AddEnemies(waveData, enemyA, Mathf.RoundToInt(3 * finalScale), 1.5f);
                        AddEnemies(waveData, enemyD, Mathf.RoundToInt(2 * finalScale), 1.5f);
                        break;
                }
            }

            waveData.startDelay = 1f;
            AssetDatabase.CreateAsset(waveData, $"{saveFolder}/{stageName}/{stageName}_Wave{wave}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{waveCount} WaveData ScriptableObjects generated successfully!");
    }

    private void AddEnemies(WaveData waveData, EnemyData enemy, int count, float delay)
    {
        waveData.enemies.Add(new WaveData.EnemySpawnInfo
        {
            enemyData = enemy,
            count = count,
            spawnDelay = delay
        });
    }

    public enum PatternType
    {
        Swarm,       // 다수의 약한 적
        Mixed,       // 다양한 적 조합
        EliteGuard,  // 강한 적 위주
        Rush,        // 짧은 텀으로 몰려옴
        Alternating, // 번갈아 등장
        Trickle,     // 느리게 소수 등장
        Count
    }
}
