using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultiTargetGunTower : TrackingTower
{
    protected override void AttackToTarget()
    {
        if (closestAttackTarget == null)
        {
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        if (!closestAttackTarget.gameObject.activeSelf)
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
        List<EnemyTest> enemiesInRange = GetEnemiesInRange();

        if (enemiesInRange.Count > 0)
        {
            foreach (EnemyTest enemy in enemiesInRange)
            {
                TowerWeapon weapon = weaponPool.Spawn(weaponSpawnTransform.position);
                weapon.Setup(enemy.transform, this);
            }

            towerAnim.SetBool("isAttacking", true);
        }
        else
        {
            towerAnim.SetBool("isAttacking", false);
        }
    }
}
