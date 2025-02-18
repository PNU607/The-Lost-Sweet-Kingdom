using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;

    public int gold;
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
        gold = 0;
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("Get Money");
        UpdateGoldUI();
    }

    public void UpdateGoldUI()
    {
        goldText.text = $"Gold : {gold}";
    }
}
