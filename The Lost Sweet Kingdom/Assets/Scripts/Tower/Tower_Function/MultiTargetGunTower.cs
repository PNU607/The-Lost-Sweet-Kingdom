using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MultiTargetGunTower : TrackingTower
{
    [SerializeField]
    private Vector2Int[] attackDirections = DirectionPresets.All8;
    [SerializeField]
    private Vector2 tileCheckSizeMultiplier = new Vector2(0.9f, 0.9f);

    private Tilemap attackableTilemap;

    public override void Setup(TowerData nextTowerData, int level = 1)
    {
        base.Setup(nextTowerData, level);
        attackableTilemap = TowerManager.Instance.attackableTilemap;
    }

    protected override void AttackToTarget()
    {
        if (closestAttackTarget == null || !closestAttackTarget.gameObject.activeSelf)
        {
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);
        if (distance > applyLevelData.attackRange)
        {
            attackTargets = null;
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        SetAttackAnimation();
        attackTimer = 0;
    }

    private void SetAttackAnimation()
    {
        towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        Vector3Int towerCellPos = attackableTilemap.WorldToCell(transform.position);

        foreach (Vector2Int dir in attackDirections)
        {
            for (int i = 1; i <= applyLevelData.attackRange; i++)
            {
                Vector3Int tilePos = towerCellPos + new Vector3Int(dir.x * i, dir.y * i, 0);

                if (!attackableTilemap.HasTile(tilePos))
                    continue;

                Vector3 tileWorldPos = attackableTilemap.GetCellCenterWorld(tilePos);
                Vector2 boxSize = GetTileCheckBoxSize();

                // LayerMask 없이 모든 콜라이더 검사
                Collider2D[] enemies = Physics2D.OverlapBoxAll(tileWorldPos, boxSize, 0f);

                if (enemies.Length > 0)
                {
                    foreach (var enemyCol in enemies)
                    {
                        if (enemyCol == null) continue;

                        AttackRangeTarget(enemyCol);
                    }

                    break; // 한 방향에서 가장 가까운 타일만 처리
                }
            }
        }
    }

    protected virtual void AttackRangeTarget(Collider2D enemyCol)
    {
        if (!enemyCol.gameObject.activeSelf)
            return;

        TowerWeapon weapon = weaponPool.Spawn(weaponSpawnTransform.position);
        weapon.Setup(enemyCol.transform, this);

        var health = enemyCol.GetComponent<EnemyTest>();
        if (health != null)
            health.TakeDamage(applyLevelData.attackDamage);
    }


    Vector2 GetTileCheckBoxSize()
    {
        Vector3 cellSize = attackableTilemap.cellSize;
        return new Vector2(cellSize.x * tileCheckSizeMultiplier.x, cellSize.y * tileCheckSizeMultiplier.y);
    }
}
