using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public ReRoll reRollManager;

    public int totalEnemy;
    public int waveCount = 0;
    public bool IsTransitioning { get; private set; }
    public RoundEventController RoundEvents { get; private set; }

    private void Awake()
    {
        RoundEvents = GetComponent<RoundEventController>();
        if (RoundEvents == null) RoundEvents = gameObject.AddComponent<RoundEventController>();
        if (instance == null)
        {
            instance = this;
        }

        if (reRollManager == null)
        {
            reRollManager = FindObjectOfType<ReRoll>();
        }
    }

    private void Start()
    {
        totalEnemy = CountEnemy();
    }

    private int CountEnemy()
    {
        if (waveCount >= EnemySpawner.instance.waves.Count)
        {
            return 0;
        }

        int total = 0;
        foreach (var enemy in EnemySpawner.instance.waves[waveCount].enemies)
        {
            total += enemy.count;
        }
        return total;
    }

    public void enemyCountDown()
    {
        if (IsTransitioning || totalEnemy <= 0) return;
        totalEnemy--;
        //Debug.Log($"Remain : {totalEnemy}");
        if (totalEnemy == 0)
        {
            RoundFinish();
        }
    }

    public void RoundFinish()
    {
        if (IsTransitioning || waveCount >= EnemySpawner.instance.waves.Count ||
            (BattleManager.Instance != null && BattleManager.Instance.isCleared)) return;
        IsTransitioning = true;
        StartCoroutine(FinishRound());
    }

    public void BeginWave()
    {
        totalEnemy = CountEnemy();
        RoundEvents.PrepareRound(waveCount + 1);
    }

    private IEnumerator FinishRound()
    {
        EnemySpawner.instance.isGameRunning = false;
        waveCount++;
        EnemySpawner.instance.currentWaveIndex = waveCount;
        yield return RoundEvents.AfterRound(waveCount);
        if (reRollManager != null) reRollManager.UpdateRerollData(waveCount);
        totalEnemy = CountEnemy();
        if (waveCount < EnemySpawner.instance.waves.Count)
        {
            RoundEvents.PrepareRound(waveCount + 1);
            EnemySpawner.instance.UpdateWaveText();
            if (EnemySpawner.instance.autoGameStart)
                yield return new WaitForSeconds(5f);
        }
        IsTransitioning = false;
        if (EnemySpawner.instance.autoGameStart && waveCount < EnemySpawner.instance.waves.Count)
            EnemySpawner.instance.StartGame();
    }
}
