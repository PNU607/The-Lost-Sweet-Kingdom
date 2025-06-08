using UnityEngine;
using UnityEngine.Tilemaps;

public class RangeTower : TrackingTower
{
    [SerializeField]
    private Vector2Int[] attackDirections = DirectionPresets.All8; // 공격 방향 설정 (상하좌우 + 대각선)
    [SerializeField]
    private Vector3 attackHeadOffset = new Vector3(0,1f,0);
    [SerializeField]
    private Vector2 tileCheckSizeMultiplier = new Vector2(0.9f, 0.9f);

    private Tilemap attackableTilemap;

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
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerAnim.SetBool("isAttacking", false);
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
            towerAnim.SetBool("isAttacking", false);
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
        towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        Vector3Int towerCellPos = attackableTilemap.WorldToCell(transform.position);

        foreach (Vector2Int dir in attackDirections)
        {
            for (int i = 1; i <= applyLevelData.attackRange; i++)
            {
                Vector3Int tilePos = towerCellPos + new Vector3Int(dir.x, dir.y, 0);

                if (!attackableTilemap.HasTile(tilePos))
                    continue;

                Vector3 tileWorldPos = attackableTilemap.GetCellCenterWorld(tilePos);
                Vector2 boxSize = GetTileCheckBoxSize();

                Collider2D[] enemies = Physics2D.OverlapBoxAll(tileWorldPos, boxSize, 0f, enemyLayer);

                if (enemies.Length > 0)
                {
                    foreach (var enemyCol in enemies)
                    {
                        if (enemyCol == null) continue;

                        AttackRangeTarget(enemyCol);
                    }

                    break; // 한 방향에서 첫 타일만 처리 (멀리 있는 적은 무시)
                }
            }
        }
    }

    protected virtual void AttackRangeTarget(Collider2D enemyCol)
    {
        // 머리 위 이펙트 위치
        Vector3 effectPos = enemyCol.transform.position + attackHeadOffset;

        TowerWeapon weapon = weaponPool.Spawn(effectPos);
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
