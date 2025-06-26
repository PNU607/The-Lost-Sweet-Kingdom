using System.Collections;
using UnityEngine;

public class SlowZone : TowerWeapon
{
    private float duration = 0f; // DamageZone�� ���� �ð�
    private float tickTimer = 0f;

    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);
        duration = shotTower.applyLevelData.attackDuration; // ���� �ð� ����
    }

    protected override void Update()
    {
        base.Update();

        duration -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= 1f)
        {
            tickTimer = 0f;

            Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0f, shotTower.towerBase.enemyLayer);
            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out EnemyTest enemyTest))
                {
                    StartCoroutine(SlowEnemy(enemyTest));
                }
            }
        }

        if (duration <= 0f)
            Destroy(gameObject);
    }

    /// <summary>
    /// �����ð����� ���� �̼��� ������ ����
    /// </summary>
    /// <param name="enemy">������ ��</param>
    /// <returns></returns>
    private IEnumerator SlowEnemy(EnemyTest enemy)
    {
        if (enemy == null) yield break;

        // �� �̼� ����
        enemy.SetSpeedMultiplier(1 / shotTower.applyLevelData.attackDamage, shotTower.applyLevelData.attackDuration);
    }
}
