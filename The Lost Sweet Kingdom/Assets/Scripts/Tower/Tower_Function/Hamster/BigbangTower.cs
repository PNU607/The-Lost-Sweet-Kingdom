using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BigbangTower : TrackingTower
{
    public int burstCount = 10;
    public float burstInterval = 0.2f;
    public Vector2 scaleRange = new Vector2(1f, 2f);

    private Vector3 weaponBaseScale;
    private bool weaponBaseScaleSet = false;

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
        if (closestAttackTarget != null)
        {
            StartCoroutine(BigbangBurst());

            towerBase.towerAnim.SetBool("isAttacking", true);
        }
        else
        {
            towerBase.towerAnim.SetBool("isAttacking", false);
        }
    }

    private IEnumerator BigbangBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * applyLevelData.attackRange;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

            TowerWeapon weapon = weaponPool.Spawn(spawnPos);
            weapon.Setup(null, this);

            if (!weaponBaseScaleSet)
            {
                weaponBaseScale = weapon.transform.localScale;
                weaponBaseScaleSet = true;
            }

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            weapon.transform.localScale = weaponBaseScale * scale;

            yield return new WaitForSeconds(burstInterval);
        }

        towerBase.towerAnim.SetBool("isAttacking", false);
    }
}