using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ZoneTower : TrackingTower
{
    [SerializeField]
    protected Vector2Int[] attackDirections = DirectionPresets.All8; // ���� ���� ���� (�����¿� + �밢��)
    [SerializeField]
    protected Vector2 zoneSizeMultiplier = new Vector2(0.9f, 0.9f);

    protected Tilemap attackableTilemap;

    public override void Setup(TowerData nextTowerData, int level = 1)
    {
        base.Setup(nextTowerData, level);
        attackableTilemap = TowerManager.Instance.attackableTilemap;
    }

    /// <summary>
    /// Ÿ���� ���� �߻�ü�� ����
    /// </summary>
    /// <returns></returns>
    protected override void AttackToTarget()
    {
        // Ÿ���� ������
        if (closestAttackTarget == null)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // Ÿ���� ��Ȱ��ȭ�Ǹ�
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // Ÿ�ٰ��� �Ÿ� ���
        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        // Ÿ�ٰ��� �Ÿ��� ���� �������� �ָ� ������
        if (distance > applyLevelData.attackRange)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            attackTargets = null;
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // ����
        SetAttackAnimation();
        attackTimer = 0;
    }

    /// <summary>
    /// �߻�ü ���� �� ����
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
