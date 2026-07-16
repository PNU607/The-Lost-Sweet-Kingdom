using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    /// <summary>
    /// 발사체 생성할 위치 (transform)
    /// </summary>
    public Transform weaponSpawnTransform;

    /// <summary>
    /// 타워 드래그 시 위치 보정값
    /// </summary>
    public Vector3 offset;
    /// <summary>
    /// 타워의 콜라이더
    /// </summary>
    public Collider2D towerCollider;

    /// <summary>
    /// 타워의 공격 범위 표시용 SpriteRenderer
    /// </summary>
    public SpriteRenderer rangeIndicator;

    /// <summary>
    /// 적 레이어
    /// </summary>
    public LayerMask enemyLayer;

    /// <summary>
    /// 타워의 애니메이터
    /// </summary>
    public Animator towerAnim;

    /// <summary>
    /// 타워의 스프라이트 렌더러
    /// </summary>
    public SpriteRenderer towerSprite;

    public GameObject StarArea; 
    public GameObject LevelStarObj;
}
