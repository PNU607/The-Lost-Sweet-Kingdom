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
    public LineRenderer lineRenderer;


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
            towerAnim.SetBool("isAttacking", false);
            StopLaser();
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerAnim.SetBool("isAttacking", false);
            StopLaser();
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟과의 거리 계산
        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        // 타겟과의 거리가 공격 범위보다 멀리 있으면
        if (distance > applyData.attackRange)
        {
            // 타겟 탐색 상태로 전환
            attackTargets = null;
            towerAnim.SetBool("isAttacking", false);
            StopLaser();
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 공격
        FireLaser();
        attackTimer = 0;
    }

    /// <summary>
    /// 레이저 중지
    /// </summary>
    private void StopLaser()
    {
        lineRenderer.enabled = false;
    }

    /// <summary>
    /// 레이저 발사
    /// </summary>
    private void FireLaser()
    {
        if (closestAttackTarget == null) return;

        Debug.Log("Fire Laser");
        towerAnim.SetBool("isAttacking", true);
        Vector2 startPos = transform.position;
        Vector2 direction = (closestAttackTarget.transform.position - transform.position).normalized;
        float maxDistance = applyData.attackWeaponRange;

        // 레이저의 끝 지점 설정 (레이저가 최대 사거리까지 가도록)
        Vector2 endPos = (Vector2)transform.position + direction * maxDistance;

        // LineRenderer로 레이저 시각화
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Raycast로 선상의 모든 적 찾기
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, maxDistance, enemyLayer);
        foreach (RaycastHit2D hit in hits)
        {
            EnemyTest enemy = hit.collider.GetComponent<EnemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(applyData.attackDamage);
            }
        }

        StartCoroutine(DisableLaser());
    }

    /// <summary>
    /// 레이저 LineRenderer 비활성화
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableLaser()
    {
        yield return new WaitForSeconds(0.1f);
        lineRenderer.enabled = false;
    }
}
