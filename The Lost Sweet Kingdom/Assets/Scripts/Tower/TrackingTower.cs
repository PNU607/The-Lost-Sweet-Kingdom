/* 
 * @file: TrackingTower.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 타겟을 따라 회전하는 타워 스크립트
 * @details:
 *  - 타겟 방향으로 회전하는 기능
 * @see: Tower.cs
 * @history:
 *  - 2025-02-09: TrackingTower 스크립트 최초 작성
 *  - 2025-02-22: TrackingTower class 내 Update문 virtual에서 override로 수정
 *  - 2025-03-08: 공격 방향에 따라 sprite flip 기능 추가
 */

using UnityEngine;

/* 
 * @class: TrackingTower
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 타겟을 따라 회전하는 타워 클래스
 * @details:
 *  - 타겟 방향으로 회전 기능
 * @history:
 *  - 2025-02-09: TrackingTower 클래스 최초 작성
 *  - 2025-02-22: Update문 virtual -> override로 수정
 *  - 2025-03-08: RotateToTarget 함수 내 sprite flip 기능 추가
 */
public class TrackingTower : Tower
{
    /// <summary>
    /// 타워 세팅
    /// 타워를 탐색 상태로 변경
    /// </summary>
    /// <param name="enemyManager"></param>
    public override void Setup(TowerData towerData = null)
    {
        base.Setup(towerData);
        ChangeState(TowerState.SearchTarget);
    }

    /// <summary>
    /// 업데이트
    /// 공격 타겟이 있으면 타겟 방향으로 회전
    /// </summary>
    protected override void Update()
    {
        base.Update();
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    /// <summary>
    /// 공격 타겟 방향으로 회전
    /// </summary>
    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        if (degree > -90 && degree < 90)
        {
            towerSprite.flipX = true;
        }
        else
        {
            towerSprite.flipX = false;
            degree += 180;
        }
        //Quaternion targetRotation = Quaternion.Euler(0, 0, degree);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * currentTowerData.rotationSpeed);
    }
}
