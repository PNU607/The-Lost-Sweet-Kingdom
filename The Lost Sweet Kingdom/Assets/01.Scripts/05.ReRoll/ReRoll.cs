using System;
using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;
using UnityEngine.UI;

public class ReRoll : MonoBehaviour
{
    [SerializeField]
    private List<string> rerollPoolIds = new() { "AllTowers", "Rabbit", "Squirrel" };

    [NonSerialized]
    private readonly List<ReRollData> rerollDataList = new();
    private ReRollData currentRerollData;
    private int currentRerollIndex = -1;

    public Transform unitPanel;
    public int rerollCost = 2;
    [SerializeField] private TMPro.TMP_Text freeRerollText;
    private int freeRerolls;
    private int roundFreeRerolls;
    public int FreeRerolls => freeRerolls + roundFreeRerolls;

    public void AddFreeRerolls(int count)
    {
        freeRerolls += Mathf.Max(0, count);
        UpdateFreeRerollUI();
    }

    public void SetRoundFreeRerolls(int count)
    {
        roundFreeRerolls = Mathf.Max(0, count);
        UpdateFreeRerollUI();
    }

    private void UpdateFreeRerollUI()
    {
        if (freeRerollText != null) freeRerollText.text = $"무료 리롤 {FreeRerolls}회";
    }
    public GameObject towerUIPrefab;

    private void Start()
    {
        LoadReRollPools();

        if (WaveManager.instance != null)
        {
            UpdateRerollData(WaveManager.instance.waveCount);
        }
        else
        {
            UpdateRerollData(0);
        }

        GenerateUnits();
        UpdateFreeRerollUI();
    }

    private void LoadReRollPools()
    {
        GameDataRepository.EnsureLoaded();
        rerollDataList.Clear();

        if (rerollPoolIds == null || rerollPoolIds.Count == 0)
        {
            rerollPoolIds = new List<string> { "AllTowers", "Rabbit", "Squirrel" };
        }

        foreach (string poolId in rerollPoolIds)
        {
            rerollDataList.Add(GameDataRepository.GetReRollPool(poolId));
        }
    }
    public void UpdateRerollData(int currentWaveCount)
    {
        if (rerollDataList == null || rerollDataList.Count == 0)
        {
            Debug.LogError("ReRoll Data List가 비어있습니다.");
            return;
        }

        int newIndex = currentWaveCount / 10;

        if (newIndex >= rerollDataList.Count)
        {
            newIndex = rerollDataList.Count - 1;
        }

        if (currentRerollIndex != newIndex)
        {
            currentRerollIndex = newIndex;
            currentRerollData = rerollDataList[currentRerollIndex];
            Debug.Log($"ReRoll Data Index가 {currentRerollIndex}로 변경되었습니다. (현재 웨이브: {currentWaveCount})");
        }
        else if (currentRerollData == null)
        {
            currentRerollData = rerollDataList[newIndex];
        }
    }

    public void OnReRollButton()
    {
        if (RoundEventController.IsBlocking) return;
        if (FreeRerolls > 0 || GoldManager.instance.gold >= rerollCost)
        {
            SoundObject _soundObject;
            _soundObject = Sound.Play("EnemyAttacked", false);
            if (roundFreeRerolls > 0) roundFreeRerolls--;
            else if (freeRerolls > 0) freeRerolls--;
            else GoldManager.instance.SpendGold(rerollCost);
            UpdateFreeRerollUI();
            GenerateUnits();
        }
        else
        {
            Debug.Log("Not Enough Money!");
        }
    }

    private void GenerateUnits()
    {
        foreach (Transform child in unitPanel)
        {
            Destroy(child.gameObject);
        }

        HorizontalLayoutGroup layout = unitPanel.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            layout.enabled = true;

        for (int i = 0; i < 5; i++)
        {
            Unit randomUnit = GetRandomUnitBasedOnProbability();

            if (randomUnit?.towerData != null)
            {
                GameObject towerObj = Instantiate(towerUIPrefab, unitPanel);
                TowerDragDrop towerDragDrop = towerObj.GetComponent<TowerDragDrop>();
                towerDragDrop.SetUp(randomUnit.towerData);
            }
        }

        StartCoroutine(DisableLayoutAfterFrame());
    }

    private IEnumerator DisableLayoutAfterFrame()
    {
        yield return null;
        HorizontalLayoutGroup layout = unitPanel.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
            layout.enabled = false;
    }

    private Unit GetRandomUnitBasedOnProbability()
    {
        if (currentRerollData == null || currentRerollData.units.Count == 0)
        {
            return null;
        }

        float totalProbability = 0f;

        foreach (Unit unit in currentRerollData.units)
        {
            totalProbability += unit.spawnProbability;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (Unit unit in currentRerollData.units)
        {
            cumulativeProbability += unit.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return unit;
            }
        }

        return currentRerollData.units[0];
    }
}
