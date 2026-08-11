using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZoneTower : TrackingTower
{
    [SerializeField]
    protected Vector2Int[] attackDirections = DirectionPresets.All8; // 공격 방향 설정 (상하좌우 + 대각선)
    [SerializeField]
    protected Vector2 zoneSizeMultiplier = new Vector2(0.9f, 0.9f);

    protected Tilemap attackableTilemap;

    public override void Setup(TowerData nextTowerData, int level = 1)
    {
        base.Setup(nextTowerData, level);
        attackableTilemap = TowerManager.Instance.attackableTilemap;
    }

    /// <summary>
    /// 타겟을 향해 발사체를 생성
    /// </summary>
    /// <returns></returns>
    protected override void AttackToTarget()
    {
        // 타겟이 없으면
        if (closestAttackTarget == null)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟과의 거리 계산
        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        // 타겟과의 거리가 공격 범위보다 멀리 있으면
        if (distance > applyLevelData.attackRange)
        {
            // 타겟 탐색 상태로 전환
            attackTargets = null;
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 공격
        SetAttackAnimation();
        attackTimer = 0;
    }

    /// <summary>
    /// 발사체 생성 후 세팅
    /// </summary>
    private void SetAttackAnimation()
    {
        towerBase.towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        Vector3Int center = attackableTilemap.WorldToCell(transform.position);

        foreach (Vector2Int dir in attackDirections)
        {
            for (int i = 1; i <= applyLevelData.attackRange; i++)
            {
                Vector3Int tilePos = center + new Vector3Int(dir.x, dir.y, 0) * i;

                if (!attackableTilemap.HasTile(tilePos)) continue;

                Vector3 world = attackableTilemap.GetCellCenterWorld(tilePos);
                TowerWeapon zone = SpawnWeapon(world, closestAttackTarget.transform);

                Vector2 cellSize = attackableTilemap.cellSize;
                zone.transform.localScale = new Vector3(
                    cellSize.x * zoneSizeMultiplier.x,
                    cellSize.y * zoneSizeMultiplier.y,
                    1f
                );
                zone.Setup(closestAttackTarget.transform, this);

                break;
            }
        }
    }
}
