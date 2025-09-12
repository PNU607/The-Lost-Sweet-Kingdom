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
 *  - 2025-03-09: Tower Merge 기능 추가
 *  - 2025-05-06: drag 기능 수정
 */

using System.Collections.Generic;
using System.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
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
 *  - 2025-03-09: CanMerge, MergeTowers 함수 추가, TrySpawnTower 함수 내 Merge 기능 추가
 *  - 2025-05-06: drag 기능 수정
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

    public Transform objectPoolTrans;
    private GameObject weaponPoolsParent;

    /// <summary>
    /// 타워를 배치할 파일맵
    /// </summary>
    public Tilemap tilemap;
    /// <summary>
    /// 타워를 배치할 파일맵
    /// </summary>
    public Tilemap attackableTilemap;
    /// <summary>
    /// 배치된 타일의 위치, 타워들 조합
    /// </summary>
    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    /// <summary>
    /// 드래그 중인지 여부
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// 타워에 매겨지는 보너스 관리 매니저
    /// </summary>
    [SerializeField] private TowerBonusManager bonusManager;

    /// <summary>
    /// 현재 drag중인 tower
    /// </summary>
    private Tower currentDragTarget;

    /// <summary>
    /// 마지막으로 hover한 타워 오브젝트
    /// </summary>
    private GameObject lastHoveredTower;

    private LayerMask towerLayer;

    private Dictionary<GameObject, GameObjectPool<TowerWeapon>> weaponPools = new();

    /// <summary>
    /// Awake
    /// 싱글톤 세팅
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Start
    /// 메인 카메라 세팅
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;

        towerLayer = LayerMask.GetMask("Tower");

        weaponPoolsParent = new GameObject("WeaponPools");
        weaponPoolsParent.transform.SetParent(objectPoolTrans);
    }

    /// <summary>
    /// 업데이트
    /// </summary>
    void Update()
    {
        SetMouseHoverEvent();
        SetDragEvent();
    }

    private void SetMouseHoverEvent()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, towerLayer);

        if (hit.collider != null)
        {
            GameObject hovered = hit.collider.gameObject;
            if (hovered != lastHoveredTower)
            {
                lastHoveredTower = hovered;

                SoundObject _soundObject;
                _soundObject = Sound.Play("TowerMousehover", false);
                //_soundObject.SetVolume(0.3f);
            }
        }
        else
        {
            if (lastHoveredTower != null)
            {
                lastHoveredTower = null;
            }
        }
    }

    /// <summary>
    /// 타워를 배치할 수 있는 영역인지 체크 후 배치
    /// </summary>
    /// <param name="towerData">배치할 타워의 데이터</param>
    /// <param name="level">배치할 타워의 레벨</param>
    public bool TrySpawnTower(TowerData towerData, int level)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 타워 설치 가능 여부 확인
        Tower occupiedTower;
        Tower spawnTower = towerData.towerPrefab.GetComponent<Tower>();
        bool isOccupiedTile = IsTileOccupied(mousePosition, out occupiedTower);
        if (isOccupiedTile
            && CanMerge(occupiedTower, towerData))
        {
            MergeTowers(occupiedTower, spawnTower);
            return true;
        }
        // 배치 불가능한 위치라면 원래 자리로 되돌리기
        else if (isOccupiedTile || !IsBuildableTile(mousePosition))
        {
            Debug.Log("설치 불가 위치");
            return false;
        }

        SpawnTower(mousePosition, towerData, level);

        return true;
    }

    /// <summary>
    /// 타워를 생성하여 배치
    /// </summary>
    /// <param name="mousePosition">월드 좌표로 변환된 마우스 위치</param>
    /// <param name="towerData">배치할 타워의 데이터</param>
    /// <param name="level">배치할 타워의 레벨</param>
    private void SpawnTower(Vector3 mousePosition, TowerData towerData, int level)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        // 타워 생성
        GameObject clone = Instantiate(towerData.towerPrefab, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
        PlaceTower(cellPosition, clone);

        Tower tower = clone.GetComponent<Tower>();
        tower.Setup(towerData, level);
        bonusManager.RegisterTower(tower);
    }

    /// <summary>
    /// 해당 위치의 타일에 있는 타워 데이터 삭제
    /// </summary>
    /// <param name="worldPosition">제거할 타워의 타일 월드 위치</param>
    /// <returns></returns>
    public bool RemoveTower(Vector3 worldPosition)
    {
        Vector3 mousePosition = GetTilePosition(worldPosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        if (placedTowers.TryGetValue(cellPosition, out GameObject tower))
        {
            placedTowers.Remove(cellPosition);
            bonusManager.UnregisterTower(tower.GetComponent<Tower>());
            return true;
        }

        return false;
    }

    /// <summary>
    /// 해당 오브젝트와 같은 타워 오브젝트를 가진 타워 데이터 삭제
    /// </summary>
    /// <param name="towerObj">데이터를 삭제할 타워 오브젝트</param>
    /// <returns></returns>
    public bool RemoveTower(GameObject towerObj)
    {
        foreach (var placedTower in placedTowers)
        {
            if (placedTower.Value == towerObj)
            {
                placedTowers.Remove(placedTower.Key);
                bonusManager.UnregisterTower(towerObj.GetComponent<Tower>());
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 해당 위치의 타일에 있는 타워 삭제
    /// </summary>
    /// <param name="tower">삭제할 타워</param>
    public void DestroyTower(GameObject tower)
    {
        if (RemoveTower(tower))
        {
            Destroy(tower);
        }
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
        bonusManager.RegisterTower(towerObj.GetComponent<Tower>());
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
    public bool IsTileOccupied(Vector2 mousePosition, out Tower occupiedTower)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        bool isOccupied = placedTowers.ContainsKey(cellPosition);
        if (isOccupied)
        {
            occupiedTower = placedTowers[cellPosition].GetComponent<Tower>();
        }
        else
        {
            occupiedTower = null;
        }
        
        return placedTowers.ContainsKey(cellPosition);
    }

    /// <summary>
    /// 해당 위치 타일이 타워를 설치할 수 있는 타일 종류인지 여부 확인
    /// </summary>
    /// <param name="cellPosition">타워를 배치하려고 하는 타일 위치</param>
    /// <returns></returns>
    public bool IsBuildableTile(Vector2 mousePosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);
        return tilemap.GetTile(cellPosition) != null;
    }

    /// <summary>
    /// 현재 마우스 위치가 타워를 설치할 수 있는지 여부 확인
    /// </summary>
    /// <param name="mousePosition">월드 좌표로 변환된 마우스 위치</param>
    /// <returns></returns>
    public bool IsValidTowerTile(Vector2 mousePosition)
    {
        //Vector3Int cellPosition = tilemap.WorldToCell(mousePosition);

        // 해당 타일에 이미 타워가 있으면 설치 불가
        if (IsTileOccupied(mousePosition, out Tower occupiedTower))
        {
            Debug.Log("타워가 이미 존재!");

            return false;
        }
        // 해당 타일이 타워 설치 가능한 타일 종류가 아닐 경우 설치 불가
        if (!IsBuildableTile(mousePosition))
        {
            Debug.Log("타워 설치 가능 위치 아님!");
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

    public GameObject GetTowerObjOnThePosition(Vector3 towerPosition)
    {
        Tower occupiedTower;

        if (IsTileOccupied(GetTilePosition(towerPosition), out occupiedTower))
        {
            return occupiedTower.gameObject;
        }

        return null;
    }

    /// <summary>
    /// 타워를 Merge함
    /// </summary>
    /// <param name="towerA">Merge할 위치에 있는 타워</param>
    /// <param name="towerB">Merge할 위치로 이동하는 타워</param>
    /// <returns></returns>
    public Tower MergeTowers(Tower towerA, Tower towerB)
    {
        //if (!CanMerge(towerA, towerB)) return null;

        TowerData nextTowerData = towerA.CurrentTowerData;

        // 기존 타워 제거
        DestroyTower(towerB.gameObject);

        // 새로운 타워 생성 (오브젝트 풀링 사용 가능)
        bonusManager.UnregisterTower(towerA);
        towerA.Setup(nextTowerData, towerA.towerLevel + 1);
        bonusManager.RegisterTower(towerA);

        Debug.Log(towerA.name + " Merge 성공, Level " + towerA.towerLevel);

        return towerA;
    }

    /// <summary>
    /// Merge 가능 여부 확인
    /// </summary>
    /// <param name="towerA">Merge할 위치에 있는 타워</param>
    /// <param name="towerB">Merge할 위치로 이동하는 타워</param>
    /// <returns></returns>
    public bool CanMerge(Tower towerA, Tower towerB)
    {
        // 같은 종류의 타워인지, 같은 레벨인지 확인
        if (towerA.CurrentTowerData != towerB.CurrentTowerData)
            return false;

        // 최대 레벨인지 확인
        if (towerA.CurrentTowerData != null && towerA.CurrentTowerData.levelDatas.Length == towerA.towerLevel)
            return false;

        return true;
    }

    /// <summary>
    /// Merge 가능 여부 확인
    /// </summary>
    /// <param name="towerA">Merge할 위치에 있는 타워</param>
    /// <param name="towerBData">Merge할 위치로 이동하는 타워의 데이터</param>
    /// <returns></returns>
    public bool CanMerge(Tower towerA, TowerData towerBData)
    {
        // 같은 종류의 타워인지, 같은 레벨인지 확인
        if (towerA.CurrentTowerData != towerBData)
            return false;

        // 최대 레벨인지 확인
        if (towerA.CurrentTowerData != null && towerA.CurrentTowerData.levelDatas.Length == towerA.towerLevel)
            return false;

        return true;
    }

    /// <summary>
    /// 기존 Tower의 Mouse 이벤트 처
    /// </summary>
    private void SetDragEvent()
    {
        // UI 위에 클릭한 경우 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int layerMask = LayerMask.GetMask("Tower");
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, layerMask);
            if (hit.collider != null)
            {
                currentDragTarget = hit.collider.gameObject.GetComponentInParent<Tower>();
                isDragging = true;

                // 가상의 OnMouseDown 대체
                currentDragTarget.OnMouseDownEvent();
            }
        }

        if (Input.GetMouseButton(0) && isDragging && currentDragTarget != null)
        {
            // 가상의 OnMouseDrag 대체
            currentDragTarget.OnMouseDragEvent();
        }

        if (Input.GetMouseButtonUp(0) && isDragging && currentDragTarget != null)
        {
            // 가상의 OnMouseUp 대체
            currentDragTarget.OnMouseUpEvent();

            currentDragTarget = null;
            isDragging = false;
        }
    }

    public TowerWeapon GetWeapon(GameObject weaponPrefab)
    {
        if (!weaponPools.ContainsKey(weaponPrefab))
        {
            GameObject currentWeaponPools = new GameObject(weaponPrefab.name + " Pools");
            currentWeaponPools.transform.SetParent(weaponPoolsParent.transform);

            TowerWeapon weapon = weaponPrefab.GetComponent<TowerWeapon>();
            var newPool = new GameObjectPool<TowerWeapon>(weapon, 1000, currentWeaponPools.transform);
            weaponPools[weaponPrefab] = newPool;
        }

        TowerWeapon poolWeapon = weaponPools[weaponPrefab].Spawn();
        return poolWeapon;
    }

    public void ReturnWeapon(GameObject weaponPrefab, TowerWeapon weapon)
    {
        //weapon.gameObject.SetActive(false);
        weaponPools[weaponPrefab].Release(weapon);
    }
}