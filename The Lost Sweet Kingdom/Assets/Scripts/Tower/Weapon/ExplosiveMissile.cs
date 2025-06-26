/* 
 * @file: ExplosiveMissile
 * @author: 서지혜
 * @date: 2025-03-23
 * @brief: GunTower에서 발사하는 폭발하는 유도탄 발사체 스크립트
 * @details:
 *  - 가장 가까운 적 방향으로 직선 이동하여 적과 충돌하면 주위 일정 범위까지의 적들에게 데미지를 입힘
 * @see: GunTower.cs
 * @history:
 *  - 2025-03-23: ExplosiveMissile 스크립트 최초 작성
 */

using System.Collections;
using UnityEngine;

/* 
 * @class: ExplosiveMissile
 * @author: 서지혜
 * @date: 2025-03-23
 * @brief: GunTower에서 발사하는 폭발하는 유도탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 일정 범위 내의 적들에게 데미지 입히는 기능
 * @history:
 *  - 2025-03-23: ExplosiveMissile 클래스 최초 작성
 */
public class ExplosiveMissile : Missile
{
    /// <summary>
    /// 폭발 범위 표시용 SpriteRenderer
    /// </summary>
    public SpriteRenderer explosionRangeIndicator;

    public Vector3 startTargetPosition;
    private float threshold = 0.1f;

    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);
        startTargetPosition = target.transform.position;
        explosionRangeIndicator.enabled = false;
        UpdateRangeIndicator();
    }

    /// <summary>
    /// 일정 범위의 적들에게 데미지를 줌
    /// </summary>
    protected override void Attack(Collider2D collision = null)
    {
        isAttackStopped = true;
        explosionRangeIndicator.enabled = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, shotTower.applyLevelData.attackWeaponRange, shotTower.towerBase.enemyLayer);
        foreach (Collider2D col in colliders)
        {
            EnemyTest enemy = col.GetComponent<EnemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(shotTower.applyLevelData.attackDamage);
            }
        }
        ReleaseWeapon();
    }

    protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, startTargetPosition) < threshold)
        {
            Attack();
        }
    }

    protected override void SetDirection()
    {
        //base.SetDirection();

        if (!isAttackStopped)
        {
            direction = (startTargetPosition - transform.position).normalized;
        }
        else
        {
            direction = Vector3.zero;
        }
    }

    /// <summary>
    /// 폭발 범위가 표시되는 Indicator의 크기 세팅
    /// </summary>
    private void UpdateRangeIndicator()
    {
        if (explosionRangeIndicator != null)
        {
            explosionRangeIndicator.transform.localScale = new Vector3(shotTower.applyLevelData.attackWeaponRange * 2, shotTower.applyLevelData.attackWeaponRange * 2, 1);
        }
    }

    protected override void ReleaseWeapon()
    {
        StartCoroutine(ReleaseBulletWithExplosion(shotTower.applyLevelData.attackDuration));
    }

    private IEnumerator ReleaseBulletWithExplosion(float explosionTime)
    {
        yield return new WaitForSeconds(explosionTime);

        base.ReleaseWeapon();
        explosionRangeIndicator.enabled = false;
        isAttackStopped = false;
    }
}
