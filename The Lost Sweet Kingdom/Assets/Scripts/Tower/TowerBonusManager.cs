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

    public HashSet<TowerColor> bonusAppliedColor = new HashSet<TowerColor>();
    public HashSet<TowerType> bonusAppliedType = new HashSet<TowerType>();

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

        bonusAppliedColor.Clear();
        bonusAppliedType.Clear();

        //if (bonusNameText != null)
        //{
        //    bonusNameText.text = string.Empty;
        //    foreach (var group in groupedByColor)
        //    {
        //        bonusAppliedColor.Add(group.FirstOrDefault().CurrentTowerData.towerColor);
        //        foreach (var tower in group)
        //        {
        //            tower.ApplyBonus(TowerBonus.SameTowerColor);
        //        }
        //        bonusNameText.text += group.Key + " Color Bonus\n";
        //    }
        //    bonusNameText.text += "\n";

        //    foreach (var group in groupedByType)
        //    {
        //        bonusAppliedType.Add(group.FirstOrDefault().CurrentTowerData.towerType);
        //        foreach (var tower in group)
        //        {
        //            tower.ApplyBonus(TowerBonus.SameTowerType);
        //        }
        //        bonusNameText.text += group.Key + " Type Bonus ";
        //    }
        //}
    }

    private void AddBonusIcons()
    {
        foreach (var colorBonus in bonusAppliedColor)
        {
            GameObject bonusIconClone = Instantiate(bonusIconPrefab, bonusPanel.transform);
            BonusData bonusData = bonusDataBase.GetBonusByName("SameTowerColor_" + colorBonus.ToString());
            bonusIconClone.GetComponent<BonusIconUI>().SetBonus(bonusData.bonusName, bonusData.bonusContent);
        }

        foreach (var typeBonus in bonusAppliedType)
        {
            GameObject bonusIconClone = Instantiate(bonusIconPrefab, bonusPanel.transform);
            BonusData bonusData = bonusDataBase.GetBonusByName("SameTowerType_" + typeBonus.ToString());
            bonusIconClone.GetComponent<BonusIconUI>().SetBonus(bonusData.bonusName, bonusData.bonusContent);
        }
    }
}

