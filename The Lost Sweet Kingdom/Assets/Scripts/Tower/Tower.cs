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
 *  - 2025-02-22: 타워의 상태 변경 기능 수정
 *  - 2025-02-23: 타워의 공격 범위 전시 기능, 타워의 이동 기능 추가
 *  - 2025-03-08: 타워의 애니메이션 추가, 타워 Merge 기능 추가
 *  - 2025-03-16: 공격 타겟 리스트로 변경
 *  - 2025-03-23: GunTower 내의 Object Pool을 가져옴. Bullet을 TowerWeapon으로 변경
 *  - 2025-03-30: target search 기능 리스트로 가져오도록 수정
 *  - 2025-04-11: 조합 보너스 기능 추가
 *  - 2025-05-01: MouseEvent 시 기능 호출 로직 수정
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// 타워의 상태
/// SearchTarget: 범위 내에 있는 적을 탐색중
/// AttackToTarget: 타겟으로 정한 적을 공격중
/// Rotate: 계속 회전중
/// None: 아무 것도 아닌 상태
/// </summary>
public enum TowerState { SearchTarget = 0, AttackToTarget, Rotate, None }


/* 
 * @class: Tower
 * @author: 서지혜
 * @date: 2025-02-08
 * @brief: 전체적인 타워의 기능(탐색, 공격, 회전 등)을 담당하는 클래스
 * @details:
 *  - 미리 저장해 둔 타워의 데이터들을 가지고 타워의 상태에 맞는 기능 실행 (탐색, 공격, 회전 등)
 * @history:
 *  - 2025-02-08: Tower 클래스 최초 작성
 *  - 2025-02-22: AttackToTarget을 IEnumerator를 void로 수정
 *  - 2025-02-23: ShowRange, UpdateRangeIndicator, OnMouseDown, OnMouseDrag, OnMouseUp 함수 추가
 *  - 2025-03-08: 타워의 애니메이션 추가, 타워 Merge 기능 추가
 *  - 2025-03-16: attackTarget -> closestAttackTarget 변수 수정
 *  - 2025-03-23: weaponPool 추가, Bullet -> TowerWeapon으로 클래스 수정
 *  - 2025-03-30: attackTarget List로 가져올 수 있도록 수정
 *  - 2025-04-11: ApplyBonus, ClearBonuses 함수 추가
 *  - 2025-05-01: OnMouseDown, OnMouseDrag, OnMouseUp 함수 뒤 Event로 붙여 TowerManager에서 Drag 관리하도록 수정
 */
public class Tower : MonoBehaviour
{
    /// <summary>
    /// 발사체 생성할 위치 (transform)
    /// </summary>
    [SerializeField]
    protected Transform weaponSpawnTransform;

    /// <summary>
    /// 현재 타워의 기본 데이터
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
    /// 현재 타워에 적용된 데이터
    /// </summary>
    public TowerLevelData applyLevelData;

    /// <summary>
    /// 현재 타워 레벨
    /// </summary>
    public int towerLevel;

    /// <summary>
    /// 발사할 발사체의 Object Pool
    /// </summary>
    protected GameObjectPool<TowerWeapon> weaponPool;

    /// <summary>
    /// 드래그 중인지 여부
    /// </summary>
    private bool isDragging = false;
    /// <summary>
    /// 타워 드래그 시 위치 보정값
    /// </summary>
    private Vector3 offset;
    /// <summary>
    /// 타워의 콜라이더
    /// </summary>
    public Collider2D towerCollider;
    /// <summary>
    /// 타워의 이전 위치
    /// </summary>
    private Vector3 prevPosition;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 현재 타워의 상태
    /// </summary>
    private TowerState currentTowerState = TowerState.SearchTarget;

    /// <summary>
    /// 이전 타워의 상태
    /// </summary>
    private TowerState prevTowerState = TowerState.None;
    /// <summary>
    /// 공격할 타겟 리스트
    /// </summary>
    protected List<EnemyTest> attackTargets = null;
    /// <summary>
    /// 가장 가까운 타겟
    /// </summary>
    protected EnemyTest closestAttackTarget = null;

    /// <summary>
    /// 타워의 공격 범위 표시용 SpriteRenderer
    /// </summary>
    public SpriteRenderer rangeIndicator;

    /// <summary>
    /// 타워의 애니메이터
    /// </summary>
    public Animator towerAnim;

    /// <summary>
    /// 타워의 스프라이트 렌더러
    /// </summary>
    protected SpriteRenderer towerSprite;

    /// <summary>
    /// 적 레이어
    /// </summary>
    public LayerMask enemyLayer;

    /// <summary>
    /// 타워에 활성화된 보너스들
    /// </summary>
    private HashSet<TowerBonus> activeBonuses = new HashSet<TowerBonus>();

    /// <summary>
    /// Start
    /// TowerWeapon Pool 생성
    /// </summary>
    protected virtual void Start()
    {
        if (currentTowerData != null && currentTowerData.weaponPrefab != null)
        {
            TowerWeapon weapon = currentTowerData.weaponPrefab.GetComponent<TowerWeapon>();
            weaponPool = new GameObjectPool<TowerWeapon>(weapon, 10);
        }
    }

    /// <summary>
    /// 타워 세팅
    /// </summary>
    /// <param name="nextTowerData"></param>
    public virtual void Setup(TowerData nextTowerData, int level = 1)
    {
        towerLevel = level;

        currentTowerData = nextTowerData;
        Debug.Log(level - 1);
        applyLevelData = currentTowerData.levelDatas[level - 1];

        var spriteLibrary = GetComponentInChildren<SpriteLibrary>();
        spriteLibrary.spriteLibraryAsset = currentTowerData.spriteLibrary;

        towerSprite = this.GetComponentInChildren<SpriteRenderer>();
        towerAnim.SetFloat("attackSpeed", 1/applyLevelData.attackCooldown);
        
        UpdateRangeIndicator();
        rangeIndicator.enabled = false; // 처음에는 숨김

        if (towerCollider == null)
        {
            towerCollider = this.GetComponentInChildren<Collider2D>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    /// <summary>
    /// 타워의 공격 범위 전시
    /// </summary>
    /// <param name="show">전시 여부</param>
    public void ShowRange(bool show)
    {
        rangeIndicator.enabled = show;
    }

    /// <summary>
    /// 공격 범위가 표시되는 Indicator의 크기 세팅
    /// </summary>
    private void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = new Vector3(applyLevelData.attackRange * 2, applyLevelData.attackRange * 2, 1);
        }
    }

    /// <summary>
    /// 마우스 버튼을 눌렀을 때
    /// </summary>
    public void OnMouseDownEvent()
    {
        // UI 위가 아닐 때만 드래그 허용
        if (!IsPointerOverUI()) 
        {
            isDragging = true;
            towerAnim.SetBool("isDragging", true);
            prevPosition = transform.position;
            offset = transform.position - GetMouseWorldPosition();
            // 이동 중 충돌 비활성화
            towerCollider.enabled = false; 

            ShowRange(true);
        }
    }

    /// <summary>
    /// 마우스를 드래그 중일 때
    /// </summary>
    public void OnMouseDragEvent()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    /// <summary>
    /// 마우스 버튼을 뗏을 때
    /// </summary>
    public void OnMouseUpEvent()
    {
        isDragging = false;
        towerAnim.SetBool("isDragging", false);
        // 이동 완료 후 충돌 활성화
        towerCollider.enabled = true;

        Vector3 movePosition = TowerManager.Instance.GetTilePosition(transform.position);
        Tower occupiedTower;
        bool isOccupiedTile = TowerManager.Instance.IsTileOccupied(movePosition, out occupiedTower);

        if (isOccupiedTile) 
        {
            // 현재 위치 타일의 타워가 자기 자신과 같을 때
            if (TowerManager.Instance.GetTowerObjOnThePosition(movePosition) == this.gameObject)
            {
                Debug.Log("타워 이동 불가");
                transform.position = prevPosition;
            }
            // Merge가 가능하면
            else if (TowerManager.Instance.CanMerge(occupiedTower, this))
            {
                TowerManager.Instance.MergeTowers(occupiedTower, this);
            }
            else
            {
                Debug.Log("타워 이동 불가");
                transform.position = prevPosition;
            }
        }
        // 배치 불가능한 위치라면 원래 자리로 되돌리기
        else if (!TowerManager.Instance.IsBuildableTile(movePosition))
        {
            //Debug.Log("타워 설치 가능 위치 아님!");

            // 원래 자리로 돌아가게 설정
            Debug.Log("타워 이동 불가");
            transform.position = prevPosition;
        }
        else
        {
            transform.position = movePosition;
            TowerManager.Instance.MoveTower(prevPosition, movePosition, this.gameObject);
            Debug.Log("타워 이동");
        }

        ShowRange(false);
    }

    /// <summary>
    /// 마우스의 현재 월드 포지션을 가져옴
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    /// <summary>
    /// 마우스가 현재 UI의 위에 있는지 확인
    /// </summary>
    /// <returns></returns>
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
        currentTowerState = newState;
    }

    /// <summary>
    /// 공격 시간 계산용 타이머
    /// </summary>
    protected float attackTimer = 0;
    /// <summary>
    /// Update
    /// 타워의 상태에 따라 실행하는 함수를 전환
    /// </summary>
    protected virtual void Update()
    {
        if (prevTowerState == currentTowerState)
        {
            return;
        }

        attackTimer += Time.deltaTime;
        if (currentTowerState == TowerState.AttackToTarget)
        {
            if (attackTimer >= applyLevelData.attackCooldown)
            {
                AttackToTarget();
            }
        }
        else
        {
            //attackTimer = 0;

            if (currentTowerState == TowerState.SearchTarget)
            {
                SearchTarget();
            }
            else if (currentTowerState == TowerState.Rotate)
            {
                Rotate();
            }
        }
    }

    /// <summary>
    /// 타겟을 탐색
    /// </summary>
    /// <returns></returns>
    protected virtual void SearchTarget()
    {
        attackTargets = GetEnemiesInRange();
        closestAttackTarget = GetClosestEnemy();

        // 공격 타겟이 있으면
        if (attackTargets != null && attackTargets.Count > 0)
        {
            ChangeState(TowerState.AttackToTarget);
        }
    }

    /// <summary>
    /// 타겟을 향해 공격
    /// </summary>
    /// <returns></returns>
    protected virtual void AttackToTarget()
    {

    }

    /// <summary>
    /// 계속해서 지속적으로 회전
    /// </summary>
    /// <returns></returns>
    protected virtual void Rotate()
    {
        transform.Rotate(0, 0, applyLevelData.rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 공격 범위 내 적들의 리스트를 반환
    /// </summary>
    /// <returns></returns>
    protected List<EnemyTest> GetEnemiesInRange()
    {
        List<EnemyTest> enemiesInRange = new List<EnemyTest>();

        // 현재 타워의 위치에서 원형의 공격 범위 내에 있는 모든 Enemy(Layer)를 가져옴
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, applyLevelData.attackRange, enemyLayer);

        foreach (Collider2D col in colliders)
        {
            EnemyTest enemy = col.GetComponent<EnemyTest>();
            if (enemy != null)
            {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }

    /// <summary>
    /// 공격 범위 내 가장 가까운 적 하나를 반환
    /// </summary>
    /// <returns></returns>
    protected EnemyTest GetClosestEnemy()
    {
        List<EnemyTest> enemies = GetEnemiesInRange();

        float closestDistance = Mathf.Infinity;
        EnemyTest closestEnemy = null;

        // 범위 내 모든 적 순회
        foreach (EnemyTest enemy in enemies)
        {
            // 각 적과 타워와의 제곱 거리 계산 (루트 연산 제거)
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            // 현재 저장된 closestDistance보다 distance가 작거나 같으면
            if (distance < closestDistance)
            {
                // 타워와 가장 가까운 적과의 거리 저장
                closestDistance = distance;
                // 타워와 가장 가까운 적을 공격 타겟으로 저장
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public virtual void Attack()
    {

    }

    /// <summary>
    /// Bullet을 Object Pool에 반환
    /// </summary>
    /// <param name="weapon"></param>
    public void ReleaseWeapon(TowerWeapon weapon)
    {
        weaponPool.Release(weapon);
    }

    /// <summary>
    /// 보너스 적용
    /// </summary>
    /// <param name="bonus">적용할 보너스</param>
    public void ApplyBonus(TowerBonus bonus)
    {
        if (!activeBonuses.Contains(bonus))
        {
            Debug.Log("조합 보너스 적용");
            activeBonuses.Add(bonus);

            switch (bonus)
            {
                case TowerBonus.SameTowerColor:
                    Debug.Log("SameTowerColor");
                    Debug.Log("기존 타워 공격력: " + applyLevelData.attackDamage);
                    applyLevelData.attackDamage += 0.5f;
                    Debug.Log("바뀐 타워 공격력: " + applyLevelData.attackDamage);
                    break;
                case TowerBonus.SameTowerType:
                    Debug.Log("SameTowerType");
                    Debug.Log("기존 타워 범위: " + applyLevelData.attackRange);
                    applyLevelData.attackRange += 0.2f;
                    Debug.Log("바뀐 타워 범위: " + applyLevelData.attackRange);
                    break;
            }
        }
    }

    /// <summary>
    /// 보너스 초기화
    /// </summary>
    public void ClearBonuses()
    {
        activeBonuses.Clear();
        applyLevelData = currentTowerData.levelDatas[towerLevel - 1];
    }

    /// <summary>
    /// Tower 종료 시 모든 코루틴 정지
    /// </summary>
    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }
}
