using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public static class GameDataRepository
{
    private static readonly Dictionary<string, TowerData> Towers =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, EnemyData> Enemies =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, WaveData> Waves =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, List<WaveData>> WavesByStage =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, ReRollData> ReRollPools =
        new(StringComparer.OrdinalIgnoreCase);

    public static bool IsLoaded { get; private set; }
    public static RoundEventCatalog EventCatalog { get; private set; }
    public static string LoadedFolderPath { get; private set; }

    public static IReadOnlyDictionary<string, TowerData> TowerData => Towers;
    public static IReadOnlyDictionary<string, EnemyData> EnemyData => Enemies;
    public static IReadOnlyDictionary<string, WaveData> WaveData => Waves;

    public static void EnsureLoaded()
    {
        if (!IsLoaded)
        {
            LoadAll();
        }
    }

    public static void LoadAll(string folderPath = null)
    {
        string resolvedFolder = string.IsNullOrWhiteSpace(folderPath)
            ? ResolveExcelFolderPath()
            : Path.GetFullPath(folderPath);

        ExcelData loaded = ExcelLoader.LoadAllExcelFiles(
            new ExcelData(),
            resolvedFolder,
            recursive: false);

        BuildRuntimeData(loaded, resolvedFolder);
    }

    public static TowerData GetTower(string towerId)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(towerId) || !Towers.TryGetValue(towerId, out TowerData data))
        {
            throw new KeyNotFoundException($"TowerData를 찾을 수 없습니다: '{towerId}'");
        }
        return data;
    }

    public static EnemyData GetEnemy(string enemyId)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(enemyId) || !Enemies.TryGetValue(enemyId, out EnemyData data))
        {
            throw new KeyNotFoundException($"EnemyData를 찾을 수 없습니다: '{enemyId}'");
        }
        return data;
    }

    public static List<WaveData> GetWavesForStage(string stageId)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(stageId) || !WavesByStage.TryGetValue(stageId, out List<WaveData> waves))
        {
            throw new KeyNotFoundException($"WaveData 스테이지를 찾을 수 없습니다: '{stageId}'");
        }
        return new List<WaveData>(waves);
    }

    public static ReRollData GetReRollPool(string poolId)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(poolId) || !ReRollPools.TryGetValue(poolId, out ReRollData pool))
        {
            throw new KeyNotFoundException($"ReRoll 풀을 찾을 수 없습니다: '{poolId}'");
        }
        return pool;
    }

    public static void ValidateRequiredAssets()
    {
        EnsureLoaded();

        foreach (TowerData tower in Towers.Values)
        {
            RequireAsset<Sprite>(tower.iconAssetName, $"TowerData/{tower.towerId}/icon");
            RequireAsset<GameObject>(tower.towerPrefabAssetName, $"TowerData/{tower.towerId}/towerPrefab");
            if (!string.IsNullOrWhiteSpace(tower.weaponPrefabAssetName))
            {
                RequireAsset<GameObject>(tower.weaponPrefabAssetName, $"TowerData/{tower.towerId}/weaponPrefab");
            }
            RequireAsset<SpriteLibraryAsset>(
                tower.spriteLibraryAssetName,
                $"TowerData/{tower.towerId}/spriteLibrary");
        }

        foreach (EnemyData enemy in Enemies.Values)
        {
            RequireAsset<GameObject>(enemy.enemyPrefabAssetName, $"EnemyData/{enemy.enemyId}/enemyPrefab");
            RequireAsset<SpriteLibraryAsset>(
                enemy.spriteLibraryAssetName,
                $"EnemyData/{enemy.enemyId}/spriteLibrary");
            if (!string.IsNullOrWhiteSpace(enemy.damagedSpriteLibraryAssetName))
            {
                RequireAsset<SpriteLibraryAsset>(
                    enemy.damagedSpriteLibraryAssetName,
                    $"EnemyData/{enemy.enemyId}/damagedSpriteLibrary");
            }
        }
    }

    public static string ResolveExcelFolderPath()
    {
        DirectoryInfo dataDirectory = new(Application.dataPath);
        DirectoryInfo parent = dataDirectory.Parent;
        if (parent == null)
        {
            throw new DirectoryNotFoundException(
                $"Application.dataPath의 상위 폴더를 찾을 수 없습니다: {Application.dataPath}");
        }

        return Path.Combine(parent.FullName, "ExcelData");
    }

    private static void BuildRuntimeData(ExcelData loaded, string folderPath)
    {
        var eventCatalog = new RoundEventCatalog(loaded);
        var towers = new Dictionary<string, TowerData>(loaded.towerData, StringComparer.OrdinalIgnoreCase);
        var enemies = new Dictionary<string, EnemyData>(loaded.enemyData, StringComparer.OrdinalIgnoreCase);
        var waves = new Dictionary<string, WaveData>(loaded.waveData, StringComparer.OrdinalIgnoreCase);
        var wavesByStage = new Dictionary<string, List<WaveData>>(StringComparer.OrdinalIgnoreCase);
        var reRollPools = new Dictionary<string, ReRollData>(StringComparer.OrdinalIgnoreCase);

        foreach (TowerData tower in towers.Values)
        {
            if (!loaded.towerLevelData.TryGetValue(tower.towerId, out List<TowerLevelData> levels))
            {
                throw new InvalidDataException($"TowerLevelData가 없습니다: {tower.towerId}");
            }

            levels.Sort((left, right) => left.level.CompareTo(right.level));
            if (levels.Count == 0 || levels.Select(level => level.level).Distinct().Count() != levels.Count)
            {
                throw new InvalidDataException($"TowerLevelData 레벨 구성이 올바르지 않습니다: {tower.towerId}");
            }
            tower.levelDatas = levels.ToArray();
        }

        foreach (WaveData wave in waves.Values)
        {
            if (!loaded.waveSpawnData.TryGetValue(wave.waveId, out List<WaveData.EnemySpawnInfo> spawnRows))
            {
                throw new InvalidDataException($"WaveSpawnData가 없습니다: {wave.waveId}");
            }

            spawnRows.Sort((left, right) => left.spawnOrder.CompareTo(right.spawnOrder));
            foreach (WaveData.EnemySpawnInfo spawn in spawnRows)
            {
                if (!enemies.TryGetValue(spawn.enemyId, out EnemyData enemy))
                {
                    throw new InvalidDataException(
                        $"WaveSpawnData가 존재하지 않는 enemyId를 참조합니다: {wave.waveId}/{spawn.enemyId}");
                }
                spawn.enemyData = enemy;
            }
            wave.enemies = spawnRows;

            if (!wavesByStage.TryGetValue(wave.stageId, out List<WaveData> stageWaves))
            {
                stageWaves = new List<WaveData>();
                wavesByStage[wave.stageId] = stageWaves;
            }
            stageWaves.Add(wave);
        }

        foreach (List<WaveData> stageWaves in wavesByStage.Values)
        {
            stageWaves.Sort((left, right) => left.waveNumber.CompareTo(right.waveNumber));
        }

        foreach (KeyValuePair<string, List<ReRollPoolRow>> entry in loaded.reRollPoolData)
        {
            entry.Value.Sort((left, right) => left.unitOrder.CompareTo(right.unitOrder));
            var pool = new ReRollData { poolId = entry.Key };
            foreach (ReRollPoolRow row in entry.Value)
            {
                if (!towers.TryGetValue(row.towerId, out TowerData tower))
                {
                    throw new InvalidDataException(
                        $"ReRollPoolData가 존재하지 않는 towerId를 참조합니다: {entry.Key}/{row.towerId}");
                }
                if (row.spawnProbability <= 0f)
                {
                    throw new InvalidDataException(
                        $"ReRoll 확률은 0보다 커야 합니다: {entry.Key}/{row.towerId}");
                }
                pool.units.Add(new Unit
                {
                    unitName = row.towerId,
                    towerId = row.towerId,
                    towerData = tower,
                    spawnProbability = row.spawnProbability,
                });
            }
            reRollPools[entry.Key] = pool;
        }

        Towers.Clear();
        Enemies.Clear();
        Waves.Clear();
        WavesByStage.Clear();
        ReRollPools.Clear();

        CopyTo(towers, Towers);
        CopyTo(enemies, Enemies);
        CopyTo(waves, Waves);
        CopyTo(wavesByStage, WavesByStage);
        CopyTo(reRollPools, ReRollPools);

        LoadedFolderPath = folderPath;
        EventCatalog = eventCatalog;
        IsLoaded = true;
        Debug.Log(
            $"Excel 데이터 로드 완료: Tower {Towers.Count}, Enemy {Enemies.Count}, " +
            $"Wave {Waves.Count}, ReRollPool {ReRollPools.Count}");
    }

    private static void CopyTo<T>(
        Dictionary<string, T> source,
        Dictionary<string, T> destination)
    {
        foreach (KeyValuePair<string, T> pair in source)
        {
            destination[pair.Key] = pair.Value;
        }
    }

    private static void RequireAsset<T>(string assetName, string context) where T : UnityEngine.Object
    {
        if (string.IsNullOrWhiteSpace(assetName))
        {
            throw new InvalidDataException($"에셋 이름이 비어 있습니다: {context}");
        }
        if (ResourceManager.Load<T>(assetName) == null)
        {
            throw new InvalidDataException(
                $"AssetResource에서 {typeof(T).Name}을(를) 찾을 수 없습니다: {context}='{assetName}'");
        }
    }
}
