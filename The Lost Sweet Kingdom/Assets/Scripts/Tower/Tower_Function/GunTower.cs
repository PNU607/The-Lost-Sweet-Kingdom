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
        // 타겟이 없으면
        if (closestAttackTarget == null)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟과의 거리 계산
        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        // 타겟과의 거리가 공격 범위보다 멀리 있으면
        if (distance > applyLevelData.attackRange)
        {
            // 타겟 탐색 상태로 전환
            attackTargets = null;
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 공격
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
        //Debug.Log("SpawnWeapon");
        if (closestAttackTarget != null)
        {
            TowerWeapon weapon = TowerManager.Instance.GetWeapon(currentTowerData.weaponPrefab);
            weapon.transform.position = towerBase.weaponSpawnTransform.position;
            weapon.Setup(closestAttackTarget.transform, this);
        }
        /*else
        {
            towerBase.towerAnim.SetBool("isAttacking", false);
        }*/
    }
}
