using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Basic Info")]
    // 타워 이름
    public string towerName;
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
    // 공격 속도(초당 공격 횟수)
    public float attackCooldown;
    // 공격 지속 시간 - 지속 딜, 디버프 공격에 이용됨
    public float attackDuration;
    // 공격력
    public float attackDamage;
    // 회전 속도
    public float rotationSpeed;


    [Header("Upgrade Settings")]
    // 타워 레벨
    public int towerLevel;
    // 업그레이드할 다음 타워 데이터 (없으면 null)
    public TowerData nextUpgrade;  
}
