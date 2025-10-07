using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;

    public int gold;
    public int firstGold;
    public TextMeshProUGUI goldText;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        gold = firstGold;
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    public void SpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.Log("Not Enogh Money");
            return;
        }
        gold -= amount;
        //Debug.Log($"Spend : {amount}");
        UpdateGoldUI();

    }

    public void UpdateGoldUI()
    {
        goldText.text = $"Gold : {gold}";
    }
}
