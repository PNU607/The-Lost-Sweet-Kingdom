using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCircle : DamageZone
{
    protected override void Update()
    {
        base.Update();

        duration -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= 1f)
        {
            tickTimer = 0f;

            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            float radius = circle.radius * transform.lossyScale.x;

            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, shotTower.enemyLayer);
            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out EnemyTest enemyTest))
                {
                    enemyTest.TakeDamage(shotTower.applyLevelData.attackDamage);
                }
            }
        }

        if (duration <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
