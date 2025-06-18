using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BonusStatType
{
    AttackPower,
    AttackSpeed,
    Range,
    FireRate,
    CriticalChance
}

public enum TowerBonus
{
    SameTowerColor,
    SameTowerType,
}

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower Defense/Tower Bonus Data")]
public class BonusData : ScriptableObject
{
    [Header("Bonus Info")]
    public TowerBonus bonusType; // 보너스 종류 (예: 같은 색상, 같은 타입 등)
    public string bonusName; // 보너스 이름
    public string bonusContent; // 보너스 설명
    public BonusStatType statAffected; // 공격력, 사거리, 공격속도 등
    public float value;           // 변화량 (예: +10%, +2 등)
    public Sprite bonusIcon; // 보너스 아이콘
}
