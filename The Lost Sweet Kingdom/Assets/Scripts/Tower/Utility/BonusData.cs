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
    public TowerBonus bonusType; // ���ʽ� ���� (��: ���� ����, ���� Ÿ�� ��)
    public string bonusName; // ���ʽ� �̸�
    public string bonusContent; // ���ʽ� ����
    public BonusStatType statAffected; // ���ݷ�, ��Ÿ�, ���ݼӵ� ��
    public float value;           // ��ȭ�� (��: +10%, +2 ��)
    public Sprite bonusIcon; // ���ʽ� ������
}
