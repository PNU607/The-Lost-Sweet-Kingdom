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
 *  - 2025-02-18: Tower 배치 기능 수정
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* 
 * @class: TowerManager
 * @author: 서지혜
 * @date: 2025-02-08
 * @brief: Tower의 생성 및 배치를 담당하는 클래스
 * @details:
 *  - 타워 생성 및 배치, 타워 세팅 기능
 * @history:
 *  - 2025-02-08: TowerManager 클래스 최초 작성
 *  - 2025-02-18: PlaceTower, RemoveTower, IsTileOccupied 함수 추가 및 수정
 */
public class TowerManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤용 instance
    /// </summary>
    public static TowerManager Instance;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 타워를 배치할 파일맵
    /// </summary>
    public Tilemap tilemap;
    /// <summary>
    /// 배치된 타일의 위치, 타워들 조합
    /// </summary>
    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    /// <summary>
    /// Awake
    /// 싱글톤 세팅
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Start
    /// 메인 카메라 세팅
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;

    }

    /// <summary>
    /// 업데이트
    /// </summary>
    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        //{
        //    PlaceTower();
        //}
    }

    /// <summary>
    /// 타워를 배치할 수 있는 영역인지 체크 후 배치
    /// </summary>
    /// <param name="towerPrefab">배치할 타워의 프리팹</param>
    public bool TrySpawnTower(GameObject towerPrefab)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // 타워 설치 가능 여부 확인
        if (!IsValidTowerTile(mousePosition))
        {
            return false;
        }

        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        SpawnTower(cellPosition, towerPrefab);

        return true;
    }

    /// <summary>
    /// 타워를 생성하여 배치
    /// </summary>
    /// <param name="cellPosition">타워를 배치하려고 하는 타일 위치</param>
    /// <param name="towerPrefab">배치할 타워의 프리팹</param>
    private void SpawnTower(Vector3Int cellPosition, GameObject towerPrefab = null)
    {
        // 타워 생성
        GameObject clone = Instantiate(towerPrefab, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
        PlaceTower(cellPosition, clone);

        Tower tower = clone.GetComponent<Tower>();
        tower.Setup();
    }

    /// <summary>
    /// 해당 위치의 타일에 있는 타워 데이터 삭제
    /// </summary>
    /// <param name="worldPosition">제거할 타워의 타일 월드 위치</param>
    /// <returns></returns>
    public bool RemoveTower(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (placedTowers.TryGetValue(cellPosition, out GameObject tower))
        {
            placedTowers.Remove(cellPosition);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 위치의 타일에 있는 타워 삭제
    /// </summary>
    /// <param name="tower">삭제할 타워</param>
    public void DestroyTower(GameObject tower)
    {
        Destroy(tower);
        RemoveTower(tower.transform.position);
    }

    /// <summary>
    /// 타워를 해당 위치로 이동
    /// </summary>
    /// <param name="prevPosition">이동하기 전 위치</param>
    /// <param name="nextPosition">이동할 위치</param>
    /// <param name="towerObj">이동할 타워의 오브젝트</param>
    public void MoveTower(Vector3 prevPosition, Vector3 nextPosition, GameObject towerObj)
    {
        RemoveTower(prevPosition);

        Vector3Int nextCellPosition = tilemap.WorldToCell(nextPosition);
        PlaceTower(nextCellPosition, towerObj);
    }

    /// <summary>
    /// 타워를 배치
    /// </summary>
    /// <param name="cellPosition">타워를 배치할 셀의 위치</param>
    /// <param name="towerObj">배치할 타워 오브젝트</param>
    private void PlaceTower(Vector3Int cellPosition, GameObject towerObj)
    {
        placedTowers[cellPosition] = towerObj;
    }

    /// <summary>
    /// 해당 위치 타일에 타워가 있는지 여부 확인
    /// </summary>
    /// <param name="cellPosition">타워를 배치하려고 하는 타일 위치</param>
    /// <returns></returns>
    private bool IsTileOccupied(Vector3Int cellPosition)
    {
        return placedTowers.ContainsKey(cellPosition);
    }

    /// <summary>
    /// 해당 위치 타일이 타워를 설치할 수 있는 타일 종류인지 여부 확인
    /// </summary>
    /// <param name="cellPosition">타워를 배치하려고 하는 타일 위치</param>
    /// <returns></returns>
    private bool IsBuildableTile(Vector3Int cellPosition)
    {
        return tilemap.GetTile(cellPosition) != null;
    }

    /// <summary>
    /// 현재 마우스 위치가 타워를 설치할 수 있는지 여부 확인
    /// </summary>
    /// <param name="mousePosition">월드 좌표로 변환된 마우스 위치</param>
    /// <returns></returns>
    public bool IsValidTowerTile(Vector2 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        // 해당 타일에 이미 타워가 있으면 설치 불가
        if (IsTileOccupied(cellPosition))
        {
            //Debug.Log("타워가 이미 존재합니다!");

            return false;
        }
        // 해당 타일이 타워 설치 가능한 타일 종류가 아닐 경우 설치 불가
        if (!IsBuildableTile(cellPosition))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 마우스 위치에 있는 타일의 중점값을 가져옴
    /// </summary>
    /// <param name="mousePosition">월드 좌표로 변환된 마우스 위치</param>
    /// <returns></returns>
    public Vector3 GetTilePosition(Vector2 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        return tilemap.GetCellCenterWorld(cellPosition);
    }
}