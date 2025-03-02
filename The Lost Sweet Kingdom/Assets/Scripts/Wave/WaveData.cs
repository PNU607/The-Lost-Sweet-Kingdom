using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave Data")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnDelay;
    }

    public List<EnemySpawnInfo> enemies;
    public float startDelay = 1f;
}
