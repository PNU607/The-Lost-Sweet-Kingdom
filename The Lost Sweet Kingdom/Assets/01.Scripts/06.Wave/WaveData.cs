using System;
using System.Collections.Generic;

[Serializable]
public class WaveData
{
    [Serializable]
    public class EnemySpawnInfo
    {
        public string waveId;
        public int spawnOrder;
        public string enemyId;
        [ExcelParer(ignore: true)]
        public EnemyData enemyData;
        public int count;
        public float spawnDelay;

        public string Key() => waveId;
    }

    public string waveId;
    public string stageId;
    public int waveNumber;
    public float startDelay = 1f;
    [ExcelParer(ignore: true)]
    public List<EnemySpawnInfo> enemies = new();

    public string Key() => waveId;
}
