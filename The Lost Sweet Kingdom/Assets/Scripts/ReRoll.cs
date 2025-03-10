using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReRoll : MonoBehaviour
{
    public ReRollData rerollData;
    public Transform unitPanel;
    public int rerollCost = 2;

    private void Start()
    {
        GenerateUnits();
    }

    public void OnReRollButton()
    {
        if (GoldManager.instance.gold >= rerollCost)
        {
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

        float offsetX = 380f;

        for (int i = 0; i < 5; i++)
        {
            Unit randomUnit = GetRandomUnitBasedOnProbability();

            if (randomUnit.TowerPrefab != null)
            {
                GameObject towerObj = Instantiate(randomUnit.TowerPrefab, unitPanel);
                towerObj.transform.localPosition = new Vector3(i * offsetX - 750f, 0, 0);
                RectTransform rectTransform = towerObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(300f, 700f);
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
