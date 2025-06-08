using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShootGunTower : TrackingTower
{
    public int shotsPerAttack = 10;
    public float shotInterval = 0.1f;

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

        StopAllCoroutines();
        StartCoroutine(ShootMultipleTargets());
    }

    private void SetAttackAnimation()
    {
        towerAnim.SetBool("isAttacking", true);
    }

    private IEnumerator ShootMultipleTargets()
    {
        List<EnemyTest> enemiesInRange = GetEnemiesInRange();

        if (enemiesInRange.Count == 0)
        {
            towerAnim.SetBool("isAttacking", false);
            yield break;
        }

        EnemyTest lastTarget = null;

        for (int i = 0; i < shotsPerAttack; i++)
        {
            EnemyTest target = null;

            int maxTry = 10;
            int tryCount = 0;

            while (tryCount < maxTry)
            {
                EnemyTest candidate = enemiesInRange[Random.Range(0, enemiesInRange.Count)];

                if (candidate != lastTarget)
                {
                    target = candidate;
                    break;
                }
                else
                {
                    if (Random.value < 0.1f)
                    {
                        target = candidate;
                        break;
                    }
                }
                tryCount++;
            }

            if (target == null)
                target = lastTarget ?? enemiesInRange[0];

            lastTarget = target;

            TowerWeapon weapon = weaponPool.Spawn(weaponSpawnTransform.position);
            weapon.Setup(target.transform, this);

            yield return new WaitForSeconds(shotInterval);
        }

        towerAnim.SetBool("isAttacking", false);
    }
}
