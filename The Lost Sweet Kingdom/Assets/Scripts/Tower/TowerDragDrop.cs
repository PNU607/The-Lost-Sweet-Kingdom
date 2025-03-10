/* 
 * @file: TowerDragDrop.cs
 * @author: 서지혜
 * @date: 2025-02-22
 * @brief: 타워의 드래그 앤 드랍 기능을 담당하는 UI 기능 스크립트
 * @details:
 *  - 타워가 드래그되는 동안 필요한 기능
 * @see: TowerData.cs, TowerManager.cs
 * @history:
 *  - 2025-02-22: TowerDragDrop 스크립트 최초 작성
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* 
 * @class: TowerDragDrop
 * @author: 서지혜
 * @date: 2025-02-22
 * @brief: 타워의 드래그 앤 드랍 기능을 담당하는 UI 기능 클래스
 * @details:
 *  - 타워를 드래그 시 타워 프리뷰 생성, 드래그한 위치 따라 이동, 드래그 완료된 위치에 배치 기능
 * @history:
 *  - 2025-02-22: TowerDragDrop 클래스 최초 작성
 */
public class TowerDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 드래그 중에 표시할 타워 프리뷰
    /// </summary>
    private GameObject previewTowerObj;
    /// <summary>
    /// UI의 캔버스 그룹
    /// </summary>
    private CanvasGroup canvasGroup;

    /// <summary>
    /// 타워 UI에 들어갈 이미지
    /// </summary>
    [SerializeField]
    private Image towerImage;

    /// <summary>
    /// UI에 표시할 타워의 데이터
    /// </summary>
    public TowerData currentTowerData;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 변수 세팅
    /// </summary>
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        towerImage = GetComponentInChildren<Image>();
        mainCamera = Camera.main;

        // @TODO: SetUp 함수 이후에 Reroll 시 해주도록 함
        SetUp(currentTowerData);
    }

    /// <summary>
    /// 기본 데이터 세팅
    /// </summary>
    /// <param name="towerData"></param>
    public void SetUp(TowerData towerData)
    {
        currentTowerData = towerData;
        towerImage.sprite = currentTowerData.towerIcon;
    }

    /// <summary>
    /// 타워의 드래그가 시작할 때 실행
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // UI 요소 반투명하게 처리
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // 타워 프리뷰 생성
        previewTowerObj = Instantiate(currentTowerData.towerPrefab);
        previewTowerObj.GetComponent<Collider2D>().enabled = false;

        // 드래그 중 사거리 표시 활성화
        Tower previewTower = previewTowerObj.GetComponent<Tower>();
        previewTower.Setup();
        previewTower.ShowRange(true);
    }

    /// <summary>
    /// 타워의 드래그하는 동안 실행
    /// 마우스 위치에 따라 미리보기 타워 이동
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (previewTowerObj != null)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            previewTowerObj.transform.position = mousePos;
        }
    }

    /// <summary>
    /// 타워의 드래그가 끝났을 때 실행
    /// 드래그가 끝난 위치에 타워 배치
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그한 타워 UI defuault로 되돌리기
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 드래그 끝난 위치에 타워 생성
        TowerManager.Instance.TrySpawnTower(currentTowerData.towerPrefab);

        // 타워 프리뷰 오브젝트 삭제
        Destroy(previewTowerObj);
        previewTowerObj = null;
    }
}