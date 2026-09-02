using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public enum TowerColor
{
    빨간색,
    주황색,
    노란색,
    초록색,
    파란색,
    남색,
    보라색
}

public enum TowerColorEng
{
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Navy,
    Purple
}

public enum TowerType
{
    토끼,
    햄스터,
    다람쥐,
}

public enum TowerTypeEng
{
    Rabbit,
    Hamster,
    Squirrel,
}

[Serializable]
public class TowerLevelData
{
    public string towerId;
    // 타워 레벨
    public int level;
    // 공격 사거리
    public float attackRange;
    // 무기의 공격 사거리
    public float attackWeaponRange;
    // 공격 속도(초당 공격 횟수)
    public float attackCooldown;
    // 공격 지속 시간 - 지속 딜, 디버프 공격에 이용됨
    public float attackDuration;
    // 공격력
    public float attackDamage;
    // 회전 속도
    public float rotationSpeed;

    public string Key() => towerId;

    public TowerLevelData Clone()
    {
        return (TowerLevelData)MemberwiseClone();
    }
}

[Serializable]
public class TowerData
{
    public string towerId;

    // 타워 이름
    public string towerName;
    // 타워 색상
    public TowerColor towerColor;
    // 타워 동물 타입
    public TowerType towerType;

    // 타워 가격
    public int cost;

    public string iconAssetName;
    public string towerPrefabAssetName;
    public string weaponPrefabAssetName;
    public string spriteLibraryAssetName;

    // 타워 레벨별 데이터 배열
    [ExcelParer(ignore: true)]
    public TowerLevelData[] levelDatas = Array.Empty<TowerLevelData>();

    public Sprite towerIcon => LoadAsset<Sprite>(iconAssetName);
    public GameObject towerPrefab => LoadAsset<GameObject>(towerPrefabAssetName);
    public GameObject weaponPrefab => LoadAsset<GameObject>(weaponPrefabAssetName);
    public SpriteLibraryAsset spriteLibrary => LoadAsset<SpriteLibraryAsset>(spriteLibraryAssetName);

    public string Key() => towerId;

    private static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        return string.IsNullOrWhiteSpace(assetName)
            ? null
            : ResourceManager.Load<T>(assetName);
    }
}
