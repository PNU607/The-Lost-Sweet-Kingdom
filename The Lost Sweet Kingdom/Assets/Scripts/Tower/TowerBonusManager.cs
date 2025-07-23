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

    public int colorCombinationNum = 3; // 보너스에 필요한 같은 색상 타워의 수
    public int typeCombinationNum = 7;  // 보너스에 필요한 같은 타입 타워의 수

    private HashSet<BonusData> activeBonusNames = new HashSet<BonusData>();
    private Dictionary<BonusData, GameObject> activeBonusIcons = new Dictionary<BonusData, GameObject>();

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


        HashSet<BonusData> newBonusNames = new HashSet<BonusData>();
        foreach (var group in groupedByColor)
        {
            string bonusName = string.Format("{0}_{1}"
                , TowerBonus.색상.ToString()
                , group.FirstOrDefault().CurrentTowerData.towerColor.ToString());
            BonusData bonusData = bonusDataBase.GetBonusByName(bonusName);

            newBonusNames.Add(bonusData);

            foreach (var tower in group)
            {
                tower.ApplyBonus(bonusData);
            }
        }
        foreach (var group in groupedByType)
        {
            string bonusName = string.Format("{0}_{1}"
                , TowerBonus.타입.ToString()
                , group.FirstOrDefault().CurrentTowerData.towerType.ToString());
            BonusData bonusData = bonusDataBase.GetBonusByName(bonusName);
            newBonusNames.Add(bonusData);
            foreach (var tower in group)
            {
                tower.ApplyBonus(bonusData);
            }
        }

        // 생긴 보너스
        var addedBonus = newBonusNames.Except(activeBonusNames).ToList();
        // 사라진 보너스
        var removedBonus = activeBonusNames.Except(newBonusNames).ToList();

        foreach (var bonus in addedBonus)
        {
            Debug.Log($"Added Bonus: {bonus.bonusName}");
            ShowComboIcon(bonus);
        }
        foreach (var bonus in removedBonus)
        {
            Debug.Log($"Removed Bonus: {bonus.bonusName}");
            HideComboIcon(bonus);
        }

        activeBonusNames = newBonusNames;
    }

    public void ShowComboIcon(BonusData bonusData)
    {
        if (activeBonusIcons.ContainsKey(bonusData)) return;

        GameObject bonusIconClone = Instantiate(bonusIconPrefab, bonusPanel.transform);
        bonusIconClone.GetComponent<BonusIconUI>().SetBonus(bonusData);


        activeBonusIcons.Add(bonusData, bonusIconClone);
    }

    public void HideComboIcon(BonusData bonusData)
    {
        if (activeBonusIcons.TryGetValue(bonusData, out var icon))
        {
            Destroy(icon);
            activeBonusIcons.Remove(bonusData);
        }
    }
}

