using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum BonusStatType
{
    공격력,
    공격속도,
    공격범위,
}

public enum TowerBonus
{
    색상,
    타입,
}

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower Defense/Tower Bonus Data")]
public class BonusData : ScriptableObject
{
    [Header("Bonus Info")]
    public TowerBonus bonusType; // 보너스 종류 (예: 같은 색상, 같은 타입 등)
    [HideInInspector]
    public string bonusTitle; // 보너스 이름
    public string bonusName;
    public string bonusContent; // 보너스 설명
    public BonusStatType statAffected; // 공격력, 사거리, 공격속도 등
    public float value;           // 변화량 (예: +10%)
    public Sprite bonusIcon; // 보너스 아이콘

#if UNITY_EDITOR
    private void OnValidate()
    {
        string bonusCase = string.Empty;

        if (this.name == TowerColorEng.Red.ToString())
        {
            bonusCase = TowerColor.빨간색.ToString();
        }
        else if (this.name == TowerColorEng.Orange.ToString())
        {
            bonusCase = TowerColor.주황색.ToString();
        }
        else if (this.name == TowerColorEng.Yellow.ToString())
        {
            bonusCase = TowerColor.노란색.ToString();
        }
        else if (this.name == TowerColorEng.Green.ToString())
            {
            bonusCase = TowerColor.초록색.ToString();
        }
        else if (this.name == TowerColorEng.Blue.ToString())
        {
            bonusCase = TowerColor.파란색.ToString();
        }
        else if (this.name == TowerColorEng.Navy.ToString())
        {
            bonusCase = TowerColor.남색.ToString();
        }
        else if (this.name == TowerColorEng.Purple.ToString())
        {
            bonusCase = TowerColor.보라색.ToString();
        }
        else if (this.name == TowerTypeEng.Rabbit.ToString())
        {
            bonusCase = TowerType.토끼.ToString();
        }
        else if (this.name == TowerTypeEng.Hamster.ToString())
        {
            bonusCase = TowerType.햄스터.ToString();
        }
        else if (this.name == TowerTypeEng.Squirrel.ToString())
        {
            bonusCase = TowerType.다람쥐.ToString();
        }

        bonusTitle = string.Format("{0}_{1}", bonusType.ToString(), bonusCase);
        if (bonusType.Equals(TowerBonus.색상))
        {
            bonusName = string.Format(bonusCase + " 색상 보너스");
            bonusContent = string.Format("{0} 타워의 {1}개 이상 조합으로 {2}이/가 {3}% 증가합니다.",
                bonusCase, 
                TowerBonusManager.Instance?.colorCombinationNum == null ? 3 : TowerBonusManager.Instance?.colorCombinationNum, 
                statAffected, (value * 100));
        }
        else if (bonusType.Equals(TowerBonus.타입))
        {
            bonusName = string.Format(bonusCase + " 타입 보너스");
            bonusContent = string.Format("{0} 타워의 {1}개 이상 조합으로 {2}이/가 {3}% 증가합니다.",
                bonusCase, 
                TowerBonusManager.Instance?.typeCombinationNum == null ? 7 : TowerBonusManager.Instance?.typeCombinationNum, 
                statAffected, (value * 100));
        }
    }
#endif
}
