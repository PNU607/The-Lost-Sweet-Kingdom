using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCircle : DamageZone
{
    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);
        duration = shotTower.applyLevelData.attackDuration; // 지속 시간 설정
    }

    protected override void AttackNoTarget()
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        float radius = circle.radius * transform.localScale.x;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, shotTower.towerBase.enemyLayer);
        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyTest))
            {
                enemyTest.TakeDamage(shotTower.applyLevelData.attackDamage);
            }
        }
    }
}
