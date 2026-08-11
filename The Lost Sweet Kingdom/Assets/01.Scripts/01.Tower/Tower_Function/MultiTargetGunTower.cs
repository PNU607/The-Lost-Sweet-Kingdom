using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultiTargetGunTower : TrackingTower
{
    protected override void AttackToTarget()
    {
        if (closestAttackTarget == null)
        {
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        if (!closestAttackTarget.gameObject.activeSelf)
        {
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        if (distance > applyLevelData.attackRange)
        {
            attackTargets = null;
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        SetAttackAnimation();
        attackTimer = 0;
    }

    private void SetAttackAnimation()
    {
        towerBase.towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        List<Enemy> enemiesInRange = GetEnemiesInRange();

        if (enemiesInRange.Count > 0)
        {
            foreach (Enemy enemy in enemiesInRange)
            {
                TowerWeapon weapon = TowerManager.Instance.GetWeapon(currentTowerData.weaponPrefab);
                weapon.transform.position = towerBase.weaponSpawnTransform.position;
                weapon.Setup(enemy.transform, this);
            }

            towerBase.towerAnim.SetBool("isAttacking", true);
        }
        else
        {
            towerBase.towerAnim.SetBool("isAttacking", false);
        }
    }
}
