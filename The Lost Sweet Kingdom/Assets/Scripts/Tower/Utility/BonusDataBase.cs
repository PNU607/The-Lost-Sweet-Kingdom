using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower Defense/Tower Bonus DataBase")]
public class BonusDataBase : ScriptableObject
{
    public List<BonusData> bonusDatas;

    // �̸����� �������� �Լ�
    public BonusData GetBonusByName(string _bonusName)
    {
        return bonusDatas.FirstOrDefault(b => b.bonusTitle == _bonusName);
    }
}
