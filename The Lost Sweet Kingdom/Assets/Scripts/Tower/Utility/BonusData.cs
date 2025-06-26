using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    [HideInInspector]
    public string bonusTitle; // ���ʽ� �̸�
    public string bonusName;
    public string bonusContent; // ���ʽ� ����
    public BonusStatType statAffected; // ���ݷ�, ��Ÿ�, ���ݼӵ� ��
    public float value;           // ��ȭ�� (��: +10%, +2 ��)
    public Sprite bonusIcon; // ���ʽ� ������

#if UNITY_EDITOR
    private void OnValidate()
    {
        bonusTitle = string.Format("{0}_{1}", bonusType.ToString(), this.name);
        if (bonusType.Equals(TowerBonus.SameTowerColor))
            bonusName = string.Format(this.name + " Color Bonus");
        else if (bonusType.Equals(TowerBonus.SameTowerType))
            bonusName = string.Format(this.name + " Type Bonus");
    }
#endif
}
