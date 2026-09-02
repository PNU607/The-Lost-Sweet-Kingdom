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
 *  - 2025-02-22: 타워의 상태 변경 기능 수정
 *  - 2025-03-08: 타워의 애니메이션 추가
 *  - 2025-03-16: 공격 타겟 리스트로 변경되면서 변수 수정
 */

using UnityEngine;
using System.Collections;

/* 
 * @class: GunTower
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 타겟을 따라 회전하며 타겟을 향해 발사체를 쏘는 타워 클래스
 * @details:
 *  - 발사체 생성 위치에서 타겟 방향으로 발사체 생성 기능
 * @history:
 *  - 2025-02-09: GunTower 클래스 최초 작성
 *  - 2025-02-22: AttackToTarget을 IEnumerator를 void로 수정
 *  - 2025-03-08: 타워의 애니메이션 추가
 *  - 2025-03-16: attackTarget -> closestAttackTarget 변수 수정
 */
public class GunTower : TrackingTower
{
    /// <summary>
    /// 타겟을 향해 발사체를 생성
    /// </summary>
    /// <returns></returns>
    protected override void AttackToTarget()
    {
        if (IsAimLocked)
        {
            return;
        }

        Enemy target = GetClosestEnemy();
        if (target == null || !target.gameObject.activeSelf)
        {
            CancelAttackAndSearch();
            return;
        }

        LockAimToTarget(target);
        SetAttackAnimation();
        attackTimer = 0;
    }

    /// <summary>
    /// 발사체 생성 후 세팅
    /// </summary>
    private void SetAttackAnimation()
    {
        towerBase.towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        Enemy target = LockedAttackTarget;

        if (target != null && target.gameObject.activeSelf)
        {
            TowerWeapon weapon = TowerManager.Instance.GetWeapon(currentTowerData.weaponPrefab);
            weapon.transform.position = towerBase.weaponSpawnTransform.position;
            weapon.Setup(target.transform, this);
        }

        FinishAttackAndSearch();
    }

    private void CancelAttackAndSearch()
    {
        FinishAttackAndSearch();
    }

    private void FinishAttackAndSearch()
    {
        towerBase.towerAnim.SetBool("isAttacking", false);
        ReleaseAimLock();
        attackTargets = null;
        closestAttackTarget = null;
        ChangeState(TowerState.SearchTarget);
    }
}
