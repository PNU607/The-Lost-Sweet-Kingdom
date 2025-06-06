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
 *  - 2025-03-16: 공격 타겟 리스트로 변경되면서 변수 수정
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
 *  - 2025-03-16: attackTarget -> closestAttackTarget 변수 수정
 */
public class TrackingTower : Tower
{
    /// <summary>
    /// 타워 세팅
    /// 타워를 탐색 상태로 변경
    /// </summary>
    /// <param name="towerData"></param>
    /// <param name="level"></param>
    public override void Setup(TowerData towerData, int level = 1)
    {
        base.Setup(towerData, level);
        
        ChangeState(TowerState.SearchTarget);
    }

    /// <summary>
    /// 업데이트
    /// 공격 타겟이 있으면 타겟 방향으로 회전
    /// </summary>
    protected override void Update()
    {
        base.Update();
        if (closestAttackTarget != null)
        {
            RotateToTarget();
        }
    }

    /// <summary>
    /// 공격 타겟 방향으로 회전
    /// </summary>
    private void RotateToTarget()
    {
        float dx = closestAttackTarget.transform.position.x - transform.position.x;
        float dy = closestAttackTarget.transform.position.y - transform.position.y;

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

        if (dx > 0)
        {
            weaponSpawnTransform.localPosition = new Vector3(Mathf.Abs(weaponSpawnTransform.localPosition.x), weaponSpawnTransform.localPosition.y, weaponSpawnTransform.localPosition.z);
        }
        else
        {
            weaponSpawnTransform.localPosition = new Vector3(-Mathf.Abs(weaponSpawnTransform.localPosition.x), weaponSpawnTransform.localPosition.y, weaponSpawnTransform.localPosition.z);
        }
        //Quaternion targetRotation = Quaternion.Euler(0, 0, degree);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * currentTowerData.rotationSpeed);
    }
}
