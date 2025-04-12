using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TowerBonus
{
    SameTowerColor,
    SameTowerType,
}

public class TowerBonusManager : MonoBehaviour
{
    public static TowerBonusManager Instance;
    private readonly List<Tower> allTowers = new();

    [SerializeField]
    private int colorCombinationNum = 3;
    [SerializeField]
    private int typeCombinationNum = 7;

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

        foreach (var group in groupedByColor)
        {
            bonusAppliedColor.Add(group.FirstOrDefault().CurrentTowerData.towerColor);
            foreach (var tower in group)
            {
                tower.ApplyBonus(TowerBonus.SameTowerColor);
            }
        }

        foreach (var group in groupedByType)
        {
            bonusAppliedType.Add(group.FirstOrDefault().CurrentTowerData.towerType);
            foreach (var tower in group)
            {
                tower.ApplyBonus(TowerBonus.SameTowerType);
            }
        }
    }
}

