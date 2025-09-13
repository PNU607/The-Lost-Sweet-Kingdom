using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircularStageSelector : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 300f; // 원의 반지름
    public float animationDuration = 0.6f; // 애니메이션 시간
    public Ease animationEase = Ease.OutQuad; // 애니메이션 이징

    [Header("UI References")]
    public Button leftButton;
    public Button rightButton;
    public Transform itemParent; // 아이템들의 부모 오브젝트
    public GameObject itemPrefab; // 아이템 프리팹

    [Header("Display Settings")]
    public float centerScale = 1.2f; // 가운데 아이템의 크기
    public float sideScale = 0.8f; // 좌우 아이템의 크기
    public float centerAlpha = 1f; // 가운데 아이템의 투명도
    public float sideAlpha = 0.6f; // 좌우 아이템의 투명도

    // 데이터
    private List<StageData> allStageData = new List<StageData>();
    private List<GameObject> allItems = new List<GameObject>();
    private int currentCenterIndex = 0;
    private bool isAnimating = false;

    // 고정 위치 (중앙, 우측, 좌측)
    private Vector3 centerPos;
    private Vector3 rightPos;
    private Vector3 leftPos;

    void Start()
    {
        // 버튼 이벤트 연결
        if (leftButton != null)
            leftButton.onClick.AddListener(MoveLeft);
        if (rightButton != null)
            rightButton.onClick.AddListener(MoveRight);

        // 위치 계산
        CalculatePositions();
    }

    void CalculatePositions()
    {
        Vector3 center = itemParent != null ? itemParent.position : Vector3.zero;

        centerPos = center;
        rightPos = center + new Vector3(radius * Mathf.Sin(120f * Mathf.Deg2Rad),
                                       radius * Mathf.Cos(120f * Mathf.Deg2Rad), 0);
        leftPos = center + new Vector3(radius * Mathf.Sin(240f * Mathf.Deg2Rad),
                                      radius * Mathf.Cos(240f * Mathf.Deg2Rad), 0);
    }

    // Stage에서 호출하여 아이템들 생성
    public void CreateItems(List<StageData> stageDataList)
    {
        // 기존 아이템들 제거
        ClearAllItems();

        // 데이터 저장
        allStageData = new List<StageData>(stageDataList);

        // 모든 아이템 생성
        for (int i = 0; i < allStageData.Count; i++)
        {
            GameObject item = CreateSingleItem(allStageData[i], i);
            allItems.Add(item);
        }

        // 초기 위치 설정
        currentCenterIndex = 0;
        UpdateItemsImmediate();
    }

    GameObject CreateSingleItem(StageData stageData, int index)
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Item Prefab이 설정되지 않았습니다!");
            return null;
        }

        // 아이템 생성
        GameObject itemObj = Instantiate(itemPrefab, itemParent);

        // StageSlot 컴포넌트에 데이터 설정
        StageSlot stageSlot = itemObj.GetComponent<StageSlot>();
        if (stageSlot != null)
        {
            stageSlot.SetData(stageData);
        }

        // CanvasGroup 추가 (투명도 조절용)
        if (itemObj.GetComponent<CanvasGroup>() == null)
        {
            itemObj.AddComponent<CanvasGroup>();
        }

        // 클릭 이벤트 추가
        Button itemButton = itemObj.GetComponent<Button>();
        if (itemButton == null)
            itemButton = itemObj.AddComponent<Button>();

        int itemIndex = index;
        itemButton.onClick.AddListener(() => SelectItem(itemIndex));

        return itemObj;
    }

    void ClearAllItems()
    {
        foreach (var item in allItems)
        {
            if (item != null)
            {
                item.transform.DOKill(); // DOTween 애니메이션 중지
                DestroyImmediate(item);
            }
        }

        allItems.Clear();
        allStageData.Clear();
        currentCenterIndex = 0;
    }

    public void MoveLeft()
    {
        if (isAnimating || allItems.Count == 0) return;

        currentCenterIndex = (currentCenterIndex - 1 + allItems.Count) % allItems.Count;
        AnimateItems();
    }

    public void MoveRight()
    {
        if (isAnimating || allItems.Count == 0) return;

        currentCenterIndex = (currentCenterIndex + 1) % allItems.Count;
        AnimateItems();
    }

    void AnimateItems()
    {
        isAnimating = true;

        for (int i = 0; i < allItems.Count; i++)
        {
            GameObject item = allItems[i];
            int displayIndex = GetDisplayIndex(i);

            if (displayIndex == -1)
            {
                // 화면에서 사라져야 하는 아이템
                item.transform.DOScale(0f, animationDuration).SetEase(animationEase);
                item.GetComponent<CanvasGroup>().DOFade(0f, animationDuration).SetEase(animationEase)
                    .OnComplete(() => item.SetActive(false));
            }
            else
            {
                // 화면에 나타나야 하는 아이템
                item.SetActive(true);

                Vector3 targetPos = GetTargetPosition(displayIndex);
                float targetScale = GetTargetScale(displayIndex);
                float targetAlpha = GetTargetAlpha(displayIndex);

                // DOTween 애니메이션
                item.transform.DOMove(targetPos, animationDuration).SetEase(animationEase);
                item.transform.DOScale(targetScale, animationDuration).SetEase(animationEase);
                item.GetComponent<CanvasGroup>().DOFade(targetAlpha, animationDuration).SetEase(animationEase);
            }
        }

        // 애니메이션 완료 대기
        DOVirtual.DelayedCall(animationDuration, () => isAnimating = false);
    }

    void UpdateItemsImmediate()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            GameObject item = allItems[i];
            int displayIndex = GetDisplayIndex(i);

            if (displayIndex == -1)
            {
                // 화면에 표시되지 않는 아이템
                item.SetActive(false);
            }
            else
            {
                // 화면에 표시되는 아이템
                item.SetActive(true);
                item.transform.position = GetTargetPosition(displayIndex);
                item.transform.localScale = Vector3.one * GetTargetScale(displayIndex);
                item.GetComponent<CanvasGroup>().alpha = GetTargetAlpha(displayIndex);
            }
        }
    }

    // 아이템이 화면의 어느 위치에 있어야 하는지 계산 (-1이면 화면에 없음)
    int GetDisplayIndex(int itemIndex)
    {
        if (allItems.Count <= 3)
        {
            // 3개 이하면 모두 표시
            return itemIndex;
        }

        // 현재 중앙 아이템을 기준으로 상대적 위치 계산
        int relativeIndex = (itemIndex - currentCenterIndex + allItems.Count) % allItems.Count;

        switch (relativeIndex)
        {
            case 0: return 0; // 중앙
            case 1: return 1; // 우측
            case 2: return -1; // 숨김
            default:
                if (relativeIndex == allItems.Count - 1)
                    return 2; // 좌측
                else
                    return -1; // 숨김
        }
    }

    Vector3 GetTargetPosition(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0: return centerPos; // 중앙
            case 1: return rightPos;  // 우측
            case 2: return leftPos;   // 좌측
            default: return centerPos;
        }
    }

    float GetTargetScale(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0: return centerScale; // 중앙
            default: return sideScale;  // 좌우
        }
    }

    float GetTargetAlpha(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0: return centerAlpha; // 중앙
            default: return sideAlpha;  // 좌우
        }
    }

    // 특정 아이템을 중앙으로 선택
    public void SelectItem(int itemIndex)
    {
        if (isAnimating || itemIndex < 0 || itemIndex >= allItems.Count) return;

        currentCenterIndex = itemIndex;
        AnimateItems();
    }

    // 현재 선택된 아이템 데이터 반환
    public StageData GetCurrentStageData()
    {
        if (currentCenterIndex >= 0 && currentCenterIndex < allStageData.Count)
        {
            return allStageData[currentCenterIndex];
        }
        return null;
    }

    // 현재 선택된 아이템 GameObject 반환
    public GameObject GetCurrentItem()
    {
        if (currentCenterIndex >= 0 && currentCenterIndex < allItems.Count)
        {
            return allItems[currentCenterIndex];
        }
        return null;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    // 모든 아이템의 애니메이션 중지
    void OnDestroy()
    {
        foreach (var item in allItems)
        {
            if (item != null)
            {
                item.transform.DOKill();
            }
        }
    }
}