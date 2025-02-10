/* 
 * @file: Tower.cs
 * @author: 서지혜
 * @date: 2025-02-08
 * @brief: 전체적인 타워의 기능(탐색, 공격, 회전 등)을 담당하는 스크립트
 * @details:
 *  - 타워의 상태를 바꾸며, 상태에 해당하는 기능을 실행시킴
 * @see: TowerData.cs, EnemyManager.cs
 * @history:
 *  - 2025-02-08: Tower 스크립트 최초 작성
 */

using System.Collections;
using UnityEngine;

/// <summary>
/// 타워의 상태
/// SearchTarget: 범위 내에 있는 적을 탐색중
/// AttackToTarget: 타겟으로 정한 적을 공격중
/// Rotate: 계속 회전중
/// </summary>
public enum TowerState { SearchTarget = 0, AttackToTarget, Rotate }


/* 
 * @class: Tower
 * @author: 서지혜
 * @date: 2025-02-08
 * @brief: 전체적인 타워의 기능(탐색, 공격, 회전 등)을 담당하는 클래스
 * @details:
 *  - 미리 저장해 둔 타워의 데이터들을 가지고 타워의 상태에 맞는 기능 실행 (탐색, 공격, 회전 등)
 * @history:
 *  - 2025-02-08: Tower 클래스 최초 작성
 */
public class Tower : MonoBehaviour
{
    /// <summary>
    /// 현재 타워의 데이터
    /// </summary>
    [SerializeField]
    protected TowerData currentTowerData;
    public TowerData CurrentTowerData
    {
        get
        {
            return currentTowerData;
        }
        set
        {
            currentTowerData = value;
        }
    }

    /// <summary>
    /// 현재 Tower의 상태
    /// </summary>
    private TowerState currentTowerState = TowerState.SearchTarget;
    /// <summary>
    /// 공격할 타겟
    /// </summary>
    protected Transform attackTarget = null;
    /// <summary>
    /// 생성된 Enemy List를 가져오기 위한 Manager
    /// </summary>
    protected EnemyManager enemyManager;

    /// <summary>
    /// 타워 세팅
    /// </summary>
    /// <param name="enemyManager"></param>
    public virtual void Setup(EnemyManager enemyManager)
    {
        this.enemyManager = enemyManager;
    }

    /// <summary>
    /// 타워의 현재 상태를 변경
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(TowerState newState)
    {
        StopCoroutine(currentTowerState.ToString());

        currentTowerState = newState;

        StartCoroutine(currentTowerState.ToString());
    }

    /// <summary>
    /// 타겟을 탐색
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator SearchTarget()
    {
        while (true)
        {
            float closestDistSqr = Mathf.Infinity;

            // 모든 적을 순회하여
            for (int i = 0; i < enemyManager.EnemeyList.Count; i++)
            {
                // 각 적과 타워와의 거리 계산
                float distance = Vector3.Distance(enemyManager.EnemeyList[i].transform.position, transform.position);
                // 타워에 설정된 범위보다 distance가 작거나 같고, 현재 저장된 closestDistSqr보다 distance가 작거나 같으면
                if (distance <= currentTowerData.attackRange && distance <= closestDistSqr)
                {
                    // 타워와 가장 가까운 적과의 거리 저장
                    closestDistSqr = distance;
                    // 타워와 가장 가까운 적을 공격 타겟으로 저장
                    attackTarget = enemyManager.EnemeyList[i].transform;
                }
            }

            // 공격 타겟이 있으면
            if (attackTarget != null)
            {
                ChangeState(TowerState.AttackToTarget);
            }

            yield return null;
        }
    }

    /// <summary>
    /// 타겟을 향해 공격
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AttackToTarget()
    {
        yield return null;
    }

    /// <summary>
    /// 계속해서 지속적으로 회전
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(0, 0, currentTowerData.rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
