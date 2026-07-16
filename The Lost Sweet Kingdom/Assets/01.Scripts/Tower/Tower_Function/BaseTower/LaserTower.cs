/* 
 * @file: LaserTower
 * @author: 서지혜
 * @date: 2025-03-21
 * @brief: 관통 레이저를 쏘는 타워 스크립트
 * @details:
 *  - 가장 가까운 적을 향해 레이저를 발사해 일직선 상에 있는 적들에게 데미지를 줌
 * @see: Tower.cs, TowerData.cs
 * @history:
 *  - 2025-03-21: LaserTower 스크립트 최초 작성
 */

using System.Collections;
using UnityEngine;

/* 
 * @class: LaserTower
 * @author: 서지혜
 * @date: 2025-03-21
 * @brief: 관통 레이저를 쏘는 타워 클래스
 * @details:
 *  - 가장 가까운 적을 향해 관통 레이저 발사, 적과 일직선 상에 있는 레이저를 맞은 적들에게 데미지 입히는 기능
 * @history:
 *  - 2025-03-21: LaserTower 클래스 최초 작성
 */
public class LaserTower : TrackingTower
{
    /// <summary>
    /// 레이저 시각 효과 표시용 LineRenderer
    /// </summary>
    //public LineRenderer lineRenderer;

    public Laser2D laser;

    //protected override void Start()
    //{
    //    base.Start();

    //    lineRenderer.positionCount = 2;
    //}

    /// <summary>
    /// 타겟을 향해 레이저를 생성
    /// </summary>
    /// <returns></returns>
    protected override void AttackToTarget()
    {
        // 타겟이 없으면
        if (closestAttackTarget == null)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
            StopLaser();
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
            StopLaser();
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
            StopLaser();
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 공격
        SetAttackAnimation();
        attackTimer = 0;
    }

    /// <summary>
    /// 레이저 중지
    /// </summary>
    private void StopLaser()
    {
        // 타겟 없으면 레이저 숨기기
        //lineRenderer.enabled = false;

        if (laser != null)
            laser.gameObject.SetActive(false);
    }

    /// <summary>
    /// 레이저 발사
    /// </summary>
    private void SetAttackAnimation()
    {
        if (closestAttackTarget == null) return;

        towerBase.towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        if (closestAttackTarget == null) return;

        Vector2 startPos = towerBase.weaponSpawnTransform.position;
        Vector2 direction = (closestAttackTarget.transform.position - transform.position).normalized;
        float maxDistance = applyLevelData.attackRange;
        Vector2 endPos = startPos + direction * maxDistance;

        laser?.gameObject.SetActive(true);
        laser?.UpdateLaser(startPos, endPos);

        // 피격 판정
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, maxDistance, towerBase.enemyLayer);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(applyLevelData.attackDamage);
        }

        StartCoroutine(DisableLaser());
    }

    /// <summary>
    /// 레이저 LineRenderer 비활성화
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableLaser()
    {
        yield return new WaitForSeconds(0.4f);
        StopLaser();
    }

    // 에디터에서 시각화
    private void OnDrawGizmos()
    {
        //if (!showLaserInEditor) return;

        // 공격 범위 원
        //Gizmos.color = rangeColor;
        //Gizmos.DrawWireSphere(transform.position, applyLevelData != null ? applyLevelData.attackRange : 1f);

        // 레이저 라인
        if (closestAttackTarget != null)
        {
            Gizmos.color = Color.red;
            Vector3 start = towerBase != null ? towerBase.weaponSpawnTransform.position : transform.position;
            Vector3 end = closestAttackTarget.transform.position;
            Gizmos.DrawLine(start, end);

            // 타겟 방향 표시용 화살표
            Gizmos.DrawSphere(end, 0.1f);
        }
        //else if (debugStart != Vector2.zero && debugEnd != Vector2.zero)
        //{
        //    // 마지막 레이저 방향 표시 (타겟 없을 때)
        //    Gizmos.color = laserColor;
        //    Gizmos.DrawLine(debugStart, debugEnd);
        //    Gizmos.DrawSphere(debugEnd, 0.1f);
        //}
    }
}
