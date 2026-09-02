using System;
using System.Collections.Generic;

public class ExcelData
{
    [SheetBinding("TowerData", optional = false)]
    public Dictionary<string, TowerData> towerData = new(StringComparer.OrdinalIgnoreCase);

    [SheetBinding("TowerLevelData", optional = false)]
    public Dictionary<string, List<TowerLevelData>> towerLevelData = new(StringComparer.OrdinalIgnoreCase);

    [SheetBinding("EnemyData", optional = false)]
    public Dictionary<string, EnemyData> enemyData = new(StringComparer.OrdinalIgnoreCase);

    [SheetBinding("WaveData", optional = false)]
    public Dictionary<string, WaveData> waveData = new(StringComparer.OrdinalIgnoreCase);

    [SheetBinding("WaveSpawnData", optional = false)]
    public Dictionary<string, List<WaveData.EnemySpawnInfo>> waveSpawnData = new(StringComparer.OrdinalIgnoreCase);

    [SheetBinding("ReRollPoolData", optional = false)]
    public Dictionary<string, List<ReRollPoolRow>> reRollPoolData = new(StringComparer.OrdinalIgnoreCase);
}

public class ReRollPoolRow
{
    public string poolId;
    public int unitOrder;
    public string towerId;
    public float spawnProbability;

    public string Key() => poolId;
}
