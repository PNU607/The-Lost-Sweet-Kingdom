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
 *  - 2025-05-06: Drag 시 Tower Animation None 기능 추가
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Sound;

/* 
 * @class: TowerDragDrop
 * @author: 서지혜
 * @date: 2025-02-22
 * @brief: 타워의 드래그 앤 드랍 기능을 담당하는 UI 기능 클래스
 * @details:
 *  - 타워를 드래그 시 타워 프리뷰 생성, 드래그한 위치 따라 이동, 드래그 완료된 위치에 배치 기능
 * @history:
 *  - 2025-02-22: TowerDragDrop 클래스 최초 작성
 *  - 2025-03-17: Awake의 SetUp 비활성화 해놨습니다. ReRoll에서 호출하고 있어요
                  OnEndDrag에 Gold 소모 추가, Icon 선택불가 등 기능 추가했습니다
 *  - 2025-03-21: OnEndDrag에 잘못 설치 시, 다시 선택할 수 있도록 변경하였습니다
 *  - 2025-03-28: UI Setup에서 Name, Cost Update하게 했습니다.
 *  - 2025-05-06: Drag 시 Tower Animation None 기능 추가
 */
public class TowerDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// 드래그 중에 표시할 타워 프리뷰
    /// </summary>
    private GameObject previewTowerObj;
    /// <summary>
    /// 드래그 중인 타워
    /// </summary>
    private Tower previewTower;

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
    /// 타워 이름을 표시할 텍스트
    /// </summary>
    public TMP_Text towerNameText;

    /// <summary>
    /// 타워 이름 배경 색상을 변경할 UI 오브젝트
    /// </summary>
    private Image towerNameBackground;
    private float backgroundAlpha = 0.7f;

    /// <summary>
    /// Ui Scale 데이터
    /// </summary>
    private Vector3 originalScale;
    private float scaleUpFactor = 1.3f;

    /// <summary>
    /// 타워 비용을 표시할 텍스트
    /// </summary>
    public TMP_Text costText;

    /// <summary>
    /// 타워 비용 배경 색상을 변경할 UI 오브젝트
    /// </summary>
    private Image costBackground;
    private float costbackgroundAlpha = 0.5f;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// 변수 세팅
    /// </summary>
    private void Awake()
    {
        originalScale = transform.localScale;

        canvasGroup = GetComponent<CanvasGroup>();
        towerImage = GetComponentInChildren<Image>();
        costText = transform.Find("CostText").GetComponent<TMP_Text>();
        towerNameText = transform.Find("TowerNameText").GetComponent<TMP_Text>();
        costBackground = transform.Find("CostBackground").GetComponent<Image>();
        towerNameBackground = transform.Find("TowerNameBackground").GetComponent<Image>();
        
        mainCamera = Camera.main;

        // @TODO: SetUp 함수 이후에 Reroll 시 해주도록 함
        //SetUp(currentTowerData);
    }

    /// <summary>
    /// 기본 데이터 세팅
    /// </summary>
    /// <param name="towerData"></param>
    public void SetUp(TowerData towerData)
    {
        currentTowerData = towerData;
        towerImage.sprite = currentTowerData.towerIcon;
        costText.text = "Cost : " + currentTowerData.cost.ToString();
        towerNameText.text = currentTowerData.towerName;
        costBackground.color = GetColorFromTowerName(towerData.towerName, costbackgroundAlpha);
        towerNameBackground.color = GetColorFromTowerName(towerData.towerName, backgroundAlpha);
    }

    private Color GetColorFromTowerName(string name, float alpha)
    {
        int firstSpaceIndex = name.IndexOf(' ');
        string ColorName = name.Substring(0, firstSpaceIndex);

        alpha = Mathf.Clamp01(alpha);

        switch (ColorName.ToLower())
        {
            case "red": return new Color(1f, 0f, 0f, alpha);
            case "orange": return new Color(1f, 0.5f, 0f, alpha);
            case "yellow": return new Color(1f, 1f, 0f, alpha);
            case "green": return new Color(0f, 1f, 0f, alpha);
            case "blue": return new Color(0f, 0f, 1f, alpha);
            case "navy": return new Color(0f, 0f, 0.5f, alpha);
            case "purple": return new Color(0.5f, 0f, 0.5f, alpha);
            default: return new Color(0.5f, 0.5f, 0.5f, alpha);
        }
    }

    /// <summary>
    /// 마우스를 올렸을 시
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleUpFactor;

        transform.SetAsLastSibling();

        SoundObject _soundObject;
        _soundObject = Sound.Play("TowerUIMoushover", false);
        //_soundObject.SetVolume(0.1f);
    }

    /// <summary>
    /// 마우스를 내렸을 시
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;

        //selfCanvas.sortingOrder = originalOrder;
    }

    /// <summary>
    /// 타워의 드래그가 시작할 때 실행
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play("TowerMerge", false);
        //_soundObject.SetVolume(0.1f);

        // UI 요소 반투명하게 처리
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        // 타워 프리뷰 생성
        previewTowerObj = Instantiate(currentTowerData.towerPrefab);

        // 드래그 중 사거리 표시 활성화
        previewTower = previewTowerObj.GetComponent<Tower>();
        previewTower.Setup(currentTowerData, 1);
        previewTower.isAttackable = false; // 드래그 중에는 공격 불가
        previewTower.towerBase.towerCollider.enabled = false;
        previewTower.ShowRange(true);
        previewTower.towerBase.towerAnim.SetBool("isDragging", true);
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
        if (GoldManager.instance.gold >= currentTowerData.cost)
        {
            // 드래그 끝난 위치에 타워 생성
            // 잘못 된 위치일시, 초기화
            //TowerManager.Instance.TrySpawnTower(currentTowerData.towerPrefab);
            bool isBuildable = TowerManager.Instance.TrySpawnTower(previewTower, 1);
            if (isBuildable)
            {
                canvasGroup.alpha = 0.3f;
                GoldManager.instance.SpendGold(currentTowerData.cost);

                SoundObject _soundObject;
                _soundObject = Sound.Play("TowerInstallation", false);
                //_soundObject.SetVolume(0.1f);
            }
            else
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            previewTower.towerBase.towerAnim.SetBool("isDragging", false);

            // 타워 프리뷰 오브젝트 삭제
            Destroy(previewTowerObj);
            previewTowerObj = null;
        }
        else
        {
            Debug.Log("Not Enough Money!");

            // 타워 프리뷰 오브젝트 삭제
            Destroy(previewTowerObj);
            previewTowerObj = null;

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

}