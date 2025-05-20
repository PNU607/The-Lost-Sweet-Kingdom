using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;

public class ReRoll : MonoBehaviour
{
    public ReRollData rerollData;
    public Transform unitPanel;
    public int rerollCost = 2;
    public GameObject towerUIPrefab;

    private void Start()
    {
        GenerateUnits();
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
    }

    private Unit GetRandomUnitBasedOnProbability()
    {
        float totalProbability = 0f;
        foreach (Unit unit in rerollData.units)
        {
            totalProbability += unit.spawnProbability;
        }

        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (Unit unit in rerollData.units)
        {
            cumulativeProbability += unit.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return unit;
            }
        }

        return rerollData.units[0];
    }
}
