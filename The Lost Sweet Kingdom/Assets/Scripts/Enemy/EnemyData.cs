using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    //적 프리팹
    public GameObject enemyPrefab;

    [Header("Stats")]
    // 체력
    public int maxHealth = 3;
    // 이동 속도
    public float moveSpeed = 2.0f;
    // 처치 시 획득하는 골드
    public int goldReward;

    [Header("Anim")]
    public SpriteLibraryAsset spriteLibraryAsset;
    public SpriteLibraryAsset damagedSpriteLibraryAsset;

    [Header("Pooling")]
    public int poolSize = 1000;
}
