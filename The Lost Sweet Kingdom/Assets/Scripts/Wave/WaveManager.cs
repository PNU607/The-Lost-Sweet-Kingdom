using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public List<WaveData> waves;

    public int totalEnemy;
    public int waveCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        totalEnemy = CountEnemy();
    }

    private int CountEnemy()
    {
        int total = 0;
        foreach (var enemy in waves[waveCount].enemies)
        {
            total += enemy.count;
        }
        return total;
    }

    public void enemyCountDown()
    {
        totalEnemy--;
        Debug.Log($"Remain : {totalEnemy}");
        if (totalEnemy == 0)
        {
            RoundFinish();
        }
    }
    public void RoundFinish()
    {
        Debug.Log("Round Finish");
        EnemySpawner.instance.isGameRunning = false;
        waveCount++;
        totalEnemy = CountEnemy();
    }
}
