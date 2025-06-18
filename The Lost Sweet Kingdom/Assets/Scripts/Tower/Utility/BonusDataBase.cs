using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BonusDataBase : ScriptableObject
{
    public List<BonusData> bonusDatas;

    // �̸����� �������� �Լ�
    public BonusData GetBonusByName(string _bonusName)
    {
        return bonusDatas.FirstOrDefault(b => b.bonusName == _bonusName);
    }
}
