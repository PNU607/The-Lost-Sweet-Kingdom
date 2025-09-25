using System.Collections.Generic;
using UnityEngine;

public class OneTargetRangeTower : RangeTower
{
    protected Vector3 weaponScale = Vector3.one; // 무기의 기본 스케일

    public override void Attack()
    {
        base.Attack();

        if (candidates.Count > 0)
        {
            Enemy closest = null;
            float minDist = float.MaxValue;
            Vector3 towerPos = transform.position;

            foreach (var enemy in candidates)
            {
                float dist = Vector3.SqrMagnitude(towerPos - enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }

            if (closest != null)
            {
                // 머리 위 이펙트 위치
                Vector3 effectPos = closest.transform.position + attackHeadOffset;

                TowerWeapon weapon = SpawnWeapon(effectPos, closest.transform);
                weaponScale = weapon.transform.localScale; // 무기의 기본 스케일을 저장


                //var health = closest.GetComponent<EnemyTest>();

                //closest.TakeDamage(applyLevelData.attackDamage);
            }
        }
    }

    protected override void AttackRangeTarget(Collider2D enemyCol)
    {
        return; // 단일 타겟 공격이므로 범위 공격 로직은 사용하지 않음
    }
}
