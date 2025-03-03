using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReRoll : MonoBehaviour
{
    public ReRollData rerollData;
    public Transform unitPanel;
    public GameObject unitPrefab;
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

        float offsetX = 300f;
        Vector2 imageSize = new Vector2(600f, 2800f);

        for (int i = 0; i < 5; i++)
        {
            Unit randomUnit = GetRandomUnitBasedOnProbability();

            GameObject unitObj = Instantiate(unitPrefab, unitPanel);

            unitObj.transform.Find("UnitImage").GetComponent<Image>().sprite = randomUnit.unitSprite;

            RectTransform unitImageRect = unitObj.transform.Find("UnitImage").GetComponent<RectTransform>();
            unitImageRect.sizeDelta = imageSize;

            float xPos = i * offsetX - 600f;
            float yPos = 0f;

            unitObj.transform.localPosition = new Vector3(xPos, yPos, 0);
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
