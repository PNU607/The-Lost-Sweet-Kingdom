using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BonusDataBase : ScriptableObject
{
    public List<BonusData> bonusDatas;

    // 이름으로 가져오는 함수
    public BonusData GetBonusByName(string _bonusName)
    {
        return bonusDatas.FirstOrDefault(b => b.bonusName == _bonusName);
    }
}
