using System.Collections.Generic;
using UnityEngine;

public class OneTargetRangeTower : RangeTower
{
    protected List<EnemyTest> candidates = new List<EnemyTest>();

    public override void Attack()
    {
        base.Attack();

        if (candidates.Count > 0)
        {
            EnemyTest closest = null;
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

                TowerWeapon weapon = weaponPool.Spawn(effectPos);
                weapon.Setup(closest.transform, this);

                //var health = closest.GetComponent<EnemyTest>();

                //closest.TakeDamage(applyLevelData.attackDamage);
            }
        }
    }

    protected override void AttackRangeTarget(Collider2D enemyCol)
    {
        if (enemyCol.TryGetComponent(out EnemyTest enemy))
        {
            candidates.Add(enemy);
        }
    }
}
