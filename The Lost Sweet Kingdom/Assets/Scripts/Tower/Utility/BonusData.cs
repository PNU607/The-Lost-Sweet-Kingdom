using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum BonusStatType
{
    ���ݷ�,
    ���ݼӵ�,
    ���ݹ���,
}

public enum TowerBonus
{
    ����,
    Ÿ��,
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
    public float value;           // ��ȭ�� (��: +10%)
    public Sprite bonusIcon; // ���ʽ� ������

#if UNITY_EDITOR
    private void OnValidate()
    {
        string bonusCase = string.Empty;

        if (this.name == TowerColorEng.Red.ToString())
        {
            bonusCase = TowerColor.������.ToString();
        }
        else if (this.name == TowerColorEng.Orange.ToString())
        {
            bonusCase = TowerColor.��Ȳ��.ToString();
        }
        else if (this.name == TowerColorEng.Yellow.ToString())
        {
            bonusCase = TowerColor.�����.ToString();
        }
        else if (this.name == TowerColorEng.Green.ToString())
            {
            bonusCase = TowerColor.�ʷϻ�.ToString();
        }
        else if (this.name == TowerColorEng.Blue.ToString())
        {
            bonusCase = TowerColor.�Ķ���.ToString();
        }
        else if (this.name == TowerColorEng.Navy.ToString())
        {
            bonusCase = TowerColor.����.ToString();
        }
        else if (this.name == TowerColorEng.Purple.ToString())
        {
            bonusCase = TowerColor.�����.ToString();
        }
        else if (this.name == TowerTypeEng.Rabbit.ToString())
        {
            bonusCase = TowerType.�䳢.ToString();
        }
        else if (this.name == TowerTypeEng.Hamster.ToString())
        {
            bonusCase = TowerType.�ܽ���.ToString();
        }
        else if (this.name == TowerTypeEng.Squirrel.ToString())
        {
            bonusCase = TowerType.�ٶ���.ToString();
        }

        bonusTitle = string.Format("{0}_{1}", bonusType.ToString(), bonusCase);
        if (bonusType.Equals(TowerBonus.����))
        {
            bonusName = string.Format(bonusCase + " ���� ���ʽ�");
            bonusContent = string.Format("{0} Ÿ���� {1}�� �̻� �������� {2}��/�� {3}% �����մϴ�.",
                bonusCase, 
                TowerBonusManager.Instance?.colorCombinationNum == null ? 3 : TowerBonusManager.Instance?.colorCombinationNum, 
                statAffected, (value * 100));
        }
        else if (bonusType.Equals(TowerBonus.Ÿ��))
        {
            bonusName = string.Format(bonusCase + " Ÿ�� ���ʽ�");
            bonusContent = string.Format("{0} Ÿ���� {1}�� �̻� �������� {2}��/�� {3}% �����մϴ�.",
                bonusCase, 
                TowerBonusManager.Instance?.typeCombinationNum == null ? 7 : TowerBonusManager.Instance?.typeCombinationNum, 
                statAffected, (value * 100));
        }
    }
#endif
}
