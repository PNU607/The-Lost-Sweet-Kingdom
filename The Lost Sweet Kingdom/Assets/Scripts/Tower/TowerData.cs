using UnityEngine;
using UnityEngine.U2D.Animation;

public enum TowerColor
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
    Rabbit,
    Hamster,
}

[System.Serializable]
public class TowerLevelData
{
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
}

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Basic Info")]
    // 타워 이름
    public string towerName;
    // 타워 색상
    public TowerColor towerColor;
    // 타워 동물 타입
    public TowerType towerType;

    [Header("UI & Prefab")]
    // UI에서 표시될 타워 아이콘
    public Sprite towerIcon;
    // 배치할 타워 프리팹
    public GameObject towerPrefab;
    // 공격할 무기 프리팹
    public GameObject weaponPrefab;
    // 해당하는 타워의 Sprite Library
    public SpriteLibraryAsset spriteLibrary;

    [Header("Stats")]
    // 타워 가격
    public int cost;
    // 타워 레벨별 데이터 배열
    public TowerLevelData[] levelDatas;

#if UNITY_EDITOR
    private void OnValidate()
    {
        towerName = string.Format("{0} {1}", towerColor.ToString(), towerType.ToString());
    }
#endif
}
