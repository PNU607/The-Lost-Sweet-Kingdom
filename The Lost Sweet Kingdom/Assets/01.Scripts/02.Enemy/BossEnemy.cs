using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BossEnemy : Enemy
{
    private bool isDamagedSpriteSet = false;

    public override void TakeDamage(float damage, TowerType? attackerType)
    {
        base.TakeDamage(damage, attackerType);

        if (!isDamagedSpriteSet && hp <= currentEnemyData.maxHealth * 0.5f)
        {
            ChangeToDamagedSpriteLibrary();
            isDamagedSpriteSet = true;
        }
    }

    private void ChangeToDamagedSpriteLibrary()
    {
        if (spriteLibrary != null && currentEnemyData.damagedSpriteLibraryAsset != null)
        {
            spriteLibrary.spriteLibraryAsset = currentEnemyData.damagedSpriteLibraryAsset;
        }
    }
}
