using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TowerBonusManager : MonoBehaviour
{
    public static TowerBonusManager Instance;
    private readonly List<Tower> allTowers = new();

    public BonusDataBase bonusDataBase;

    [SerializeField]
    private GameObject bonusPanel;
    [SerializeField]
    private GameObject bonusIconPrefab;

    [SerializeField]
    private int colorCombinationNum = 3; // 보너스에 필요한 같은 색상 타워의 수
    [SerializeField]
    private int typeCombinationNum = 7;  // 보너스에 필요한 같은 타입 타워의 수

    private HashSet<string> activeBonusNames = new HashSet<string>();
    private Dictionary<string, GameObject> activeBonusIcons = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterTower(Tower tower)
    {
        allTowers.Add(tower);
        EvaluateBonuses();
    }

    public void UnregisterTower(Tower tower)
    {
        allTowers.Remove(tower);
        EvaluateBonuses();
    }

    private void EvaluateBonuses()
    {
        foreach (var tower in allTowers)
            tower.ClearBonuses();

        var groupedByColor = allTowers
            .GroupBy(t => t.CurrentTowerData.towerColor)
            .Where(g => g.Count() >= colorCombinationNum);

        var groupedByType = allTowers
            .GroupBy(t => t.CurrentTowerData.towerType)
            .Where(g => g.Count() >= typeCombinationNum);


        HashSet<string> newBonusNames = new HashSet<string>();
        foreach (var group in groupedByColor)
        {
            newBonusNames.Add(string.Format("{0}_{1}"
                , TowerBonus.SameTowerColor.ToString()
                , group.FirstOrDefault().CurrentTowerData.towerColor.ToString()));
            foreach (var tower in group)
            {
                tower.ApplyBonus(TowerBonus.SameTowerColor);
            }
        }
        foreach (var group in groupedByType)
        {
            newBonusNames.Add(string.Format("{0}_{1}"
                , TowerBonus.SameTowerType.ToString()
                , group.FirstOrDefault().CurrentTowerData.towerType.ToString()));
            foreach (var tower in group)
            {
                tower.ApplyBonus(TowerBonus.SameTowerType);
            }
        }

        // 생긴 보너스
        var addedBonus = newBonusNames.Except(activeBonusNames).ToList();
        // 사라진 보너스
        var removedBonus = activeBonusNames.Except(newBonusNames).ToList();

        foreach (var bonusName in addedBonus)
        {
            Debug.Log($"Added Bonus: {bonusName}");
            ShowComboIcon(bonusName);
        }
        foreach (var bonusName in removedBonus)
        {
            Debug.Log($"Removed Bonus: {bonusName}");
            HideComboIcon(bonusName);
        }

        activeBonusNames = newBonusNames;
    }

    public void ShowComboIcon(string bonusName)
    {
        if (activeBonusIcons.ContainsKey(bonusName)) return;

        GameObject bonusIconClone = Instantiate(bonusIconPrefab, bonusPanel.transform);
        BonusData bonusData = bonusDataBase.GetBonusByName(bonusName);
        bonusIconClone.GetComponent<BonusIconUI>().SetBonus(bonusData.bonusName, bonusData.bonusContent);


        activeBonusIcons.Add(bonusName, bonusIconClone);
    }

    public void HideComboIcon(string bonusName)
    {
        if (activeBonusIcons.TryGetValue(bonusName, out var icon))
        {
            Destroy(icon);
            activeBonusIcons.Remove(bonusName);
        }
    }
}

