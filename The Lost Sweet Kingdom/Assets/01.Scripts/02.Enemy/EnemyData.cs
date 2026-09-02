using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

[Serializable]
public class EnemyData
{
    public string enemyId;
    public string enemyName;
    public bool isBoss;

    // 체력
    public int maxHealth = 3; 
    // 체력 증가량
    public int increaseHealth = 10;
    // 이동 속도
    public float moveSpeed = 2.0f;
    // 처치 시 획득하는 골드
    public int goldReward;

    public int poolSize = 1000;

    public string enemyPrefabAssetName;
    public string spriteLibraryAssetName;
    public string damagedSpriteLibraryAssetName;

    public GameObject enemyPrefab => LoadAsset<GameObject>(enemyPrefabAssetName);
    public SpriteLibraryAsset spriteLibraryAsset => LoadAsset<SpriteLibraryAsset>(spriteLibraryAssetName);
    public SpriteLibraryAsset damagedSpriteLibraryAsset => LoadAsset<SpriteLibraryAsset>(damagedSpriteLibraryAssetName);

    public string Key() => enemyId;

    private static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        return string.IsNullOrWhiteSpace(assetName)
            ? null
            : ResourceManager.Load<T>(assetName);
    }
}
