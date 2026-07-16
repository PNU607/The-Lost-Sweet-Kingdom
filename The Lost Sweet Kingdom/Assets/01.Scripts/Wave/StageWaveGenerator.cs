using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class StageWaveGenerator : MonoBehaviour
{
    [Header("스테이지 기본 설정")]
    public string stageName = "Stage_01";
    public int totalWaves = 30;
    public string savePath = "Assets/Resources/WaveDatas/";

    [Header("난이도 곡선 설정")]
    [Range(0.05f, 0.2f)] public float difficultyStep = 0.08f;
    public Vector2 randomFactorRange = new Vector2(0.85f, 1.15f);
    public bool overwriteExisting = true;

    [Header("EnemyData Pool")]
    public List<EnemyData> allEnemies = new List<EnemyData>();

    [Header("선택된 적 구성")]
    public EnemyData enemyA;
    public EnemyData enemyB;
    public EnemyData enemyC;
    public EnemyData enemyD;
    public EnemyData boss;

    [ContextMenu("Generate Waves For Stage")]
    public void GenerateWaveData()
    {
        if (!ValidateEnemies()) return;

        System.IO.Directory.CreateDirectory($"{savePath}{stageName}/");

        for (int wave = 1; wave <= totalWaves; wave++)
        {
            var waveData = ScriptableObject.CreateInstance<WaveData>();
            waveData.enemies = new List<WaveData.EnemySpawnInfo>();

            float difficultyScale = 1f + (wave - 1) * difficultyStep;
            float randomFactor = Random.Range(randomFactorRange.x, randomFactorRange.y);
            float finalScale = difficultyScale * randomFactor;

            if (wave % 10 == 0)
            {
                waveData.enemies.Add(new WaveData.EnemySpawnInfo
                {
                    enemyData = boss,
                    count = Mathf.RoundToInt(1 + (wave / 10) * 0.3f),
                    spawnDelay = 1.5f
                });
            }
            else
            {
                PatternType pattern = (PatternType)Random.Range(0, (int)PatternType.Count);
                ApplyPattern(pattern, waveData, finalScale);
            }

            waveData.startDelay = 1f;

            string fileName = $"{stageName}_Wave{wave}.asset";
            string path = $"{savePath}/{stageName}/{fileName}";

            if (overwriteExisting)
                AssetDatabase.DeleteAsset(path);

            AssetDatabase.CreateAsset(waveData, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{stageName}: {totalWaves} WaveData 생성 완료!");
    }

    private void ApplyPattern(PatternType pattern, WaveData waveData, float scale)
    {
        switch (pattern)
        {
            case PatternType.Swarm:
                AddEnemies(waveData, enemyA, Mathf.RoundToInt(5 * scale), 0.4f);
                AddEnemies(waveData, enemyB, Mathf.RoundToInt(3 * scale), 0.5f);
                break;
            case PatternType.Mixed:
                AddEnemies(waveData, enemyA, Mathf.RoundToInt(2 * scale), 0.6f);
                AddEnemies(waveData, enemyC, Mathf.RoundToInt(2 * scale), 0.8f);
                AddEnemies(waveData, enemyD, Mathf.RoundToInt(1 * scale), 0.8f);
                break;
            case PatternType.EliteGuard:
                AddEnemies(waveData, enemyC, Mathf.RoundToInt(3 * scale), 1.2f);
                AddEnemies(waveData, enemyD, Mathf.RoundToInt(2 * scale), 1.4f);
                break;
            case PatternType.Rush:
                AddEnemies(waveData, enemyA, Mathf.RoundToInt(4 * scale), 0.25f);
                AddEnemies(waveData, enemyB, Mathf.RoundToInt(4 * scale), 0.25f);
                break;
            case PatternType.Alternating:
                for (int i = 0; i < Mathf.RoundToInt(4 * scale); i++)
                {
                    var type = (i % 2 == 0) ? enemyB : enemyC;
                    AddEnemies(waveData, type, 1, 0.5f);
                }
                break;
            case PatternType.Trickle:
                AddEnemies(waveData, enemyA, Mathf.RoundToInt(3 * scale), 1.5f);
                AddEnemies(waveData, enemyD, Mathf.RoundToInt(2 * scale), 1.5f);
                break;
        }
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

    private bool ValidateEnemies()
    {
        if (enemyA == null || enemyB == null || enemyC == null || enemyD == null || boss == null)
        {
            Debug.LogError("Enemy A~D, Boss 모두 선택해야 Wave를 생성할 수 있습니다!");
            return false;
        }
        return true;
    }

    public enum PatternType
    {
        Swarm, Mixed, EliteGuard, Rush, Alternating, Trickle, Count
    }
}
