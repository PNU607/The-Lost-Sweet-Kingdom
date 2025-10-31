using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;
using UnityEngine.UI;

public class ReRoll : MonoBehaviour
{
    public List<ReRollData> rerollDataList;
    private ReRollData currentRerollData;
    private int currentRerollIndex = -1;

    public Transform unitPanel;
    public int rerollCost = 2;
    public GameObject towerUIPrefab;

    private void Start()
    {
        if (WaveManager.instance != null)
        {
            UpdateRerollData(WaveManager.instance.waveCount);
        }
        else
        {
            UpdateRerollData(0);
        }

        GenerateUnits();
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
        if (GoldManager.instance.gold >= rerollCost)
        {
            SoundObject _soundObject;
            _soundObject = Sound.Play("EnemyAttacked", false);
            GoldManager.instance.SpendGold(rerollCost);
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

            if (randomUnit.towerData != null)
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
        if (currentRerollData == null) return null;

        float totalProbability = 0f;

        foreach (Unit unit in currentRerollData.units)
        {
            totalProbability += unit.spawnProbability;
        }

        float randomValue = Random.Range(0f, totalProbability);
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
