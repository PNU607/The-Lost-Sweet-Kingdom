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

    private bool isDragging = false;
    private Vector3 offset;
    private Collider2D towerCollider;
    private Vector3 prevPosition;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 현재 Tower의 상태
    /// </summary>
    private TowerState currentTowerState = TowerState.SearchTarget;
    /// <summary>
    /// 공격할 타겟
    /// </summary>
    protected Transform attackTarget = null;

    public SpriteRenderer rangeIndicator; // 사거리 표시용 원 SpriteRenderer

    /// <summary>
    /// 타워 세팅
    /// </summary>
    /// <param name="enemyManager"></param>
    public virtual void Setup()
    {
        UpdateRangeIndicator();
        rangeIndicator.enabled = false; // 처음에는 숨김

        if (towerCollider == null)
        {
            towerCollider = GetComponent<Collider2D>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void ShowRange(bool show)
    {
        rangeIndicator.enabled = show;
    }

    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = new Vector3(currentTowerData.attackRange * 2, currentTowerData.attackRange * 2, 1);
        }
    }

    void OnMouseDown()
    {
        if (!IsPointerOverUI()) // UI 위가 아닐 때만 드래그 허용
        {
            isDragging = true;
            prevPosition = transform.position;
            offset = transform.position - GetMouseWorldPosition();
            towerCollider.enabled = false; // 이동 중 충돌 비활성화

            ShowRange(true);
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        towerCollider.enabled = true; // 이동 완료 후 충돌 활성화

        if (!TowerManager.Instance.IsValidTowerTile(this.transform.position)) // 배치 불가능한 위치라면 원래 자리로 되돌리기
        {
            // 원래 자리로 돌아가게 설정
            transform.position = prevPosition;
        }
        else
        {
            Vector3 movePosition = TowerManager.Instance.GetTilePosition(transform.position);
            transform.position = movePosition;
            TowerManager.Instance.MoveTower(prevPosition, movePosition, this.gameObject);
        }

        ShowRange(false);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // 2D 게임이므로 Z값 고정
        return mousePos;
    }

    private bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
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
            // 현재 타워의 위치에서 원형의 공격 범위 내에 있는 모든 Enemy(Layer)를 가져옴
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, currentTowerData.attackRange, LayerMask.GetMask("Enemy"));
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            // 이전의 공격 타겟이 아직 범위 내에 있으면
            //if (attackTarget != null
            //    && hitColliders.Count() > 0
            //    && hitColliders.Select(x => x.gameObject).Contains(attackTarget.gameObject))
            //{
                // 타겟 오브젝트가 활성화 되어 있으면
            //    if (attackTarget.gameObject.activeSelf)
            //    {
                    // 계속 해당 타겟을 공격 타겟으로 하여 반복
            //        yield return null;
            //        continue;
            //    }
            //}

            // 범위 내 모든 적 순회
            foreach (Collider2D collider in hitColliders)
            {
                // 각 적과 타워와의 제곱 거리 계산 (루트 연산 제거)
                float distance = (collider.transform.position - transform.position).sqrMagnitude;
                // 현재 저장된 closestDistance보다 distance가 작거나 같으면
                if (distance < closestDistance)
                {
                    // 타워와 가장 가까운 적과의 거리 저장
                    closestDistance = distance;
                    // 타워와 가장 가까운 적을 공격 타겟으로 저장
                    closestEnemy = collider.transform;
                }
            }

            attackTarget = closestEnemy;

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

    /// <summary>
    /// Tower 종료 시 모든 코루틴 정지
    /// </summary>
    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }
}
