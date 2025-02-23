using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject previewTower; // 드래그 중에 표시할 타워 프리뷰
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Image itemImage;

    // UI에 표시할 타워의 데이터
    public TowerData currentTowerData;

    private Camera mainCamera;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        mainCamera = Camera.main;
    }

    public void SetUp(TowerData towerData)
    {
        currentTowerData = towerData;
        itemImage.sprite = currentTowerData.towerIcon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // UI 요소 반투명하게 처리
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // 타워 프리뷰 생성
        previewTower = Instantiate(currentTowerData.towerPrefab);
        previewTower.GetComponent<Collider2D>().enabled = false;

        // 드래그 중 사거리 표시 활성화
        previewTower.GetComponent<Tower>().ShowRange(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewTower != null)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            previewTower.transform.position = mousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        TowerManager.Instance.TrySpawnTower(currentTowerData.towerPrefab);
        Destroy(previewTower);
        previewTower = null;
    }
}