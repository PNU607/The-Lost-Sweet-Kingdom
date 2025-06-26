using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCircle : TowerWeapon
{
    private float duration = 0f;
    private float tickTimer = 0f;
    public float tickInterval = 1f;
    public float radius = 1f;

    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);

        duration = shotTower.applyLevelData.attackDuration;
        tickInterval = 0.5f;
        radius = shotTower.applyLevelData.attackRange;

        tickTimer = tickInterval;
    }

    protected override void Update()
    {
        duration -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            tickTimer = 0f;

            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, shotTower.towerBase.enemyLayer);
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
            ReleaseWeapon();
        }
    }
}
