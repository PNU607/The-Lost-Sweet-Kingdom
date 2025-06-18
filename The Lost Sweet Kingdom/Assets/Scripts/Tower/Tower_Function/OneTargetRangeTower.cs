using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class OneTargetRangeTower : RangeTower
{
    List<EnemyTest> candidates = new List<EnemyTest>();

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
                closest.TakeDamage(applyLevelData.attackDamage);
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
