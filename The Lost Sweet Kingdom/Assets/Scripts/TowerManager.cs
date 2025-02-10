/* 
 * @file: TowerManager.cs
 * @author: 서지혜
 * @date: 2025-02-08
 * @brief: Tower의 생성 및 배치를 담당하는 스크립트
 * @details:
 *  - 마우스 클릭 시 해당 위치가 타워 배치가 가능하면 타워를 생성함
 * @see: EnemyManager.cs, Tower.cs
 * @history:
 *  - 2025-02-08: TowerManager 스크립트 최초 작성
 */

using UnityEngine;

/* 
 * @class: TowerManager
 * @author: 서지혜
 * @date: 2025-02-08
 * @brief: Tower의 생성 및 배치를 담당하는 클래스
 * @details:
 *  - 타워 생성 및 배치, 타워 세팅 기능
 * @history:
 *  - 2025-02-08: TowerManager 클래스 최초 작성
 */
public class TowerManager : MonoBehaviour
{
    /// <summary>
    /// 생성된 Enemy List를 가져오기 위한 Manager
    /// </summary>
    [SerializeField]
    private EnemyManager enemyManager;

    /// <summary>
    /// 배치할 타워의 프리팹
    /// </summary>
    [SerializeField]
    private GameObject towerPrefab;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 타워를 배치 가능한 타일 Layer
    /// </summary>
    [SerializeField]
    private LayerMask placementLayer;

    /// <summary>
    /// Awake
    /// 메인 카메라 세팅
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// 업데이트
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            PlaceTower();
        }
    }

    /// <summary>
    /// 타워를 배치할 수 있는 영역인지 체크 후 배치
    /// </summary>
    void PlaceTower()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 월드 좌표로 변환
        RaycastHit2D hit2D = Physics2D.Raycast(mousePosition, Vector2.zero, 0f, placementLayer); // Raycast 실행

        if (hit2D.collider != null) // 배치 가능한 영역인지 체크
        {
            SpawnTower(hit2D.point);
        }
        else
        {
            Debug.Log("배치 불가능한 위치입니다!"); // 배치할 수 없는 경우 메시지 출력
        }
    }

    /// <summary>
    /// 타워 생성
    /// </summary>
    /// <param name="tileTransform"></param>
    public void SpawnTower(Vector2 tileTransform)
    {
        GameObject clone = Instantiate(towerPrefab, tileTransform, Quaternion.identity, this.transform);
        Tower tower = clone.GetComponent<Tower>();

        tower.Setup(enemyManager);
    }
}
