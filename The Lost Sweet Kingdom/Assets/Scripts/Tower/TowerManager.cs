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
    /// 배치할 타워의 프리팹
    /// </summary>
    [SerializeField]
    private GameObject towerPrefab;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 참조하는 파일맵
    /// </summary>
    public Tilemap tilemap;
    /// <summary>
    /// 배치된 타일의 위치, 타워들 조합
    /// </summary>
    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

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
        // 마우스 위치를 월드 좌표로 변환
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 해당 타일에 이미 타워가 있으면 설치 불가
        if (IsTileOccupied(mousePosition))
        {
            Debug.Log("타워가 이미 존재합니다!");
            return;
        }

        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        if (tilemap.GetTile(cellPosition) == null)
        {
            //Debug.Log("벽 타일이 아닙니다!");
            return;
        }
        // 타워 생성
        GameObject clone = Instantiate(towerPrefab, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
        placedTowers[cellPosition] = clone;

        Tower tower = clone.GetComponent<Tower>();
        tower.Setup();
    }

    /// <summary>
    /// 해당 위치의 타일에 있는 타워 제거
    /// </summary>
    /// <param name="worldPosition">제거할 타워의 타일 위치</param>
    /// <returns></returns>
    public bool RemoveTower(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (placedTowers.TryGetValue(cellPosition, out GameObject tower))
        {
            Destroy(tower);
            placedTowers.Remove(cellPosition);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 위치에 타워가 있는지 확인
    /// </summary>
    /// <param name="worldPosition">타워를 배치하려고 하는 타일 위치</param>
    /// <returns></returns>
    public bool IsTileOccupied(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return placedTowers.ContainsKey(cellPosition);
    }
}