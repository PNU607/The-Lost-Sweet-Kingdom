/* 
 * @file: GunTower.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 타겟을 따라 회전하며 타겟을 향해 발사체를 쏘는 타워 스크립트
 * @details:
 *  - 타겟 방향으로 발사체를 생성하는 기능
 * @see: TrackingTower.cs, Tower.cs, Bullet.cs
 * @history:
 *  - 2025-02-09: GunTower 스크립트 최초 작성
 */

using System.Collections;
using UnityEngine;

/* 
 * @class: GunTower
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 타겟을 따라 회전하며 타겟을 향해 발사체를 쏘는 타워 클래스
 * @details:
 *  - 발사체 생성 위치에서 타겟 방향으로 발사체 생성 기능
 * @history:
 *  - 2025-02-09: GunTower 클래스 최초 작성
 */
public class GunTower : TrackingTower
{
    /// <summary>
    /// 발사체 생성할 위치 (transform)
    /// </summary>
    Transform bulletTransform;

    /// <summary>
    /// 타워 세팅
    /// bulletTransform을 가져옴
    /// </summary>
    public override void Setup(EnemyManager enemyManager)
    {
        base.Setup(enemyManager);
        bulletTransform = transform.GetChild(0);
    }

    /// <summary>
    /// 타겟을 향해 발사체를 생성
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator AttackToTarget()
    {
        while (true)
        {
            // 타겟이 없으면
            if (attackTarget == null)
            {
                // 타겟 탐색 상태로 전환
                ChangeState(TowerState.SearchTarget);
                break;
            }

            // 타겟과의 거리 계산
            float distance = Vector3.Distance(attackTarget.position, transform.position);

            // 타겟과의 거리가 공격 범위보다 멀리 있으면
            if (distance > currentTowerData.attackRange)
            {
                // 타겟 탐색 상태로 전환
                attackTarget = null;
                ChangeState(TowerState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(currentTowerData.attackCooldown);

            // 공격
            Attack();
        }
    }

    /// <summary>
    /// 발사체 생성 후 세팅
    /// </summary>
    private void Attack()
    {
        GameObject bulletClone = Instantiate(currentTowerData.weaponPrefab, bulletTransform.position, Quaternion.identity);
        Bullet bullet = bulletClone.GetComponent<Bullet>();

        bullet.Setup(attackTarget, currentTowerData.attackDamage);
    }
}
