using UnityEngine;

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
    // 타워 레벨
    public int towerLevel;

    [Header("UI & Prefab")]
    // UI에서 표시될 타워 아이콘
    public Sprite towerIcon;
    // 배치할 타워 프리팹
    public GameObject towerPrefab;
    // 공격할 무기 프리팹
    public GameObject weaponPrefab;

    [Header("Stats")]
    // 타워 가격
    public int cost;
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

    [Header("Upgrade Settings")]
    // 업그레이드할 다음 타워 데이터 (없으면 null)
    public TowerData nextUpgrade;

#if UNITY_EDITOR
    private void OnValidate()
    {
        towerName = string.Format("{0} {1} Lv{2}", towerColor.ToString(), towerType.ToString(), towerLevel);
    }
#endif
}
