using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EruptionTower : TrackingTower
{
    private float attackInterval = 0.5f;
    private int directionCount = 8;
    private float shootDistance = 5f;

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

    private readonly Vector3[] directions = new Vector3[]
    {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
        new Vector3(1, 1, 0).normalized,
        new Vector3(1, -1, 0).normalized,
        new Vector3(-1, 1, 0).normalized,
        new Vector3(-1, -1, 0).normalized
    };

    private Vector3[] GetCircularDirections(int count)
    {
        Vector3[] dirs = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            float rad = angle * Mathf.Deg2Rad;
            dirs[i] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
        }

        return dirs;
    }

    public override void Attack()
    {
        if (closestAttackTarget == null || !closestAttackTarget.gameObject.activeSelf)
        {
            towerBase.towerAnim.SetBool("isAttacking", false);
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;

            Vector3[] directions = GetCircularDirections(directionCount);

            foreach (Vector3 dir in directions)
            {
                Vector3 shootPos = transform.position + dir * shootDistance;

                GameObject tempTargetGO = new GameObject("TempTarget");
                tempTargetGO.transform.position = shootPos;

                TowerWeapon weapon = TowerManager.Instance.GetWeapon(currentTowerData.weaponPrefab);
                weapon.transform.position = towerBase.weaponSpawnTransform.position;
                weapon.Setup(tempTargetGO.transform, this);

                Object.Destroy(tempTargetGO, 2f);
            }

            towerBase.towerAnim.SetBool("isAttacking", true);
        }
        else
        {
            towerBase.towerAnim.SetBool("isAttacking", true);
        }
    }
}
