using UnityEngine;
using UnityEngine.Tilemaps;

public class RangeTower : TrackingTower
{
    [Header("공격 설정")]
    public Vector2Int[] attackDirections = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),   // ↗
        new Vector2Int(-1, 1),  // ↖
        new Vector2Int(1, -1),  // ↘
        new Vector2Int(-1, -1)  // ↙
    };

    [Header("이펙트")]
    public GameObject attackEffectPrefab;

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
                Vector3Int checkPos = towerCellPos + new Vector3Int(dir.x, dir.y, 0) * i;

                // 공격 가능한 타일 위에 있는지 확인
                if (!attackableTilemap.HasTile(checkPos))
                    continue;

                Vector3 worldPos = attackableTilemap.GetCellCenterWorld(checkPos);
                Vector2 boxSize = GetEnemyCheckSize();

                Collider2D enemy = Physics2D.OverlapBox(worldPos, boxSize, 0f, enemyLayer);

                if (enemy != null)
                {
                    // 공격 실행
                    if (attackEffectPrefab)
                        Instantiate(attackEffectPrefab, worldPos, Quaternion.identity);

                    // 데미지 입히기
                    var health = enemy.GetComponent<EnemyTest>();
                    if (health != null)
                        health.TakeDamage(applyLevelData.attackDamage);

                    // 한 방향에서 적 하나만 공격
                    break;
                }
            }
        }
    }

    Vector2 GetEnemyCheckSize()
    {
        Vector3 cellSize = attackableTilemap.cellSize;
        return new Vector2(cellSize.x * 0.9f, cellSize.y * 0.9f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackableTilemap == null) return;

        Gizmos.color = Color.red;
        Vector3Int towerCellPos = attackableTilemap.WorldToCell(transform.position);

        foreach (Vector2Int dir in attackDirections)
        {
            for (int i = 1; i <= applyLevelData.attackRange; i++)
            {
                Vector3Int pos = towerCellPos + new Vector3Int(dir.x, dir.y, 0) * i;
                if (!attackableTilemap.HasTile(pos)) continue;

                Vector3 worldPos = attackableTilemap.GetCellCenterWorld(pos);
                Gizmos.DrawWireCube(worldPos, GetEnemyCheckSize());
            }
        }
    }
}
