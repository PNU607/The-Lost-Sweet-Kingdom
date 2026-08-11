using UnityEngine;

public class DamageZone : TowerWeapon
{
    public float duration = 0f; // DamageZone의 지속 시간
    public float tickTimer = 0f;
    public bool isAttackZone = false; // 공격 영역 여부

    public virtual void Setup(Transform target, Tower shotTower, bool isAttackZone)
    {
        this.isAttackZone = isAttackZone;
        Setup(target, shotTower);
    }

    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);
        duration = shotTower.applyLevelData.attackDuration; // 지속 시간 설정
    }

    protected override void Update()
    {
        base.Update();
        
        duration -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= 1f)
        {
            tickTimer = 0f;

            AttackNoTarget();
        }

        if (duration <= 0f)
        {
            ReleaseWeapon();
        }
    }

    protected virtual void AttackNoTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0f, shotTower.towerBase.enemyLayer);
        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyTest))
            {
                enemyTest.TakeDamage(shotTower.applyLevelData.attackDamage);
            }
        }
    }
}
