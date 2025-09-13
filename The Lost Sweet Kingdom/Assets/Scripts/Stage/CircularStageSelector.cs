using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircularStageSelector : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 300f; // ���� ������
    public float animationDuration = 0.6f; // �ִϸ��̼� �ð�
    public Ease animationEase = Ease.OutQuad; // �ִϸ��̼� ��¡

    [Header("UI References")]
    public Button leftButton;
    public Button rightButton;
    public Transform itemParent; // �����۵��� �θ� ������Ʈ
    public GameObject itemPrefab; // ������ ������

    [Header("Display Settings")]
    public float centerScale = 1.2f; // ��� �������� ũ��
    public float sideScale = 0.8f; // �¿� �������� ũ��
    public float centerAlpha = 1f; // ��� �������� ����
    public float sideAlpha = 0.6f; // �¿� �������� ����

    // ������
    private List<StageData> allStageData = new List<StageData>();
    private List<GameObject> allItems = new List<GameObject>();
    private int currentCenterIndex = 0;
    private bool isAnimating = false;

    // ���� ��ġ (�߾�, ����, ����)
    private Vector3 centerPos;
    private Vector3 rightPos;
    private Vector3 leftPos;

    void Start()
    {
        // ��ư �̺�Ʈ ����
        if (leftButton != null)
            leftButton.onClick.AddListener(MoveLeft);
        if (rightButton != null)
            rightButton.onClick.AddListener(MoveRight);

        // ��ġ ���
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

    // Stage���� ȣ���Ͽ� �����۵� ����
    public void CreateItems(List<StageData> stageDataList)
    {
        // ���� �����۵� ����
        ClearAllItems();

        // ������ ����
        allStageData = new List<StageData>(stageDataList);

        // ��� ������ ����
        for (int i = 0; i < allStageData.Count; i++)
        {
            GameObject item = CreateSingleItem(allStageData[i], i);
            allItems.Add(item);
        }

        // �ʱ� ��ġ ����
        currentCenterIndex = 0;
        UpdateItemsImmediate();
    }

    GameObject CreateSingleItem(StageData stageData, int index)
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Item Prefab�� �������� �ʾҽ��ϴ�!");
            return null;
        }

        // ������ ����
        GameObject itemObj = Instantiate(itemPrefab, itemParent);

        // StageSlot ������Ʈ�� ������ ����
        StageSlot stageSlot = itemObj.GetComponent<StageSlot>();
        if (stageSlot != null)
        {
            stageSlot.SetData(stageData);
        }

        // CanvasGroup �߰� (���� ������)
        if (itemObj.GetComponent<CanvasGroup>() == null)
        {
            itemObj.AddComponent<CanvasGroup>();
        }

        // Ŭ�� �̺�Ʈ �߰�
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
                item.transform.DOKill(); // DOTween �ִϸ��̼� ����
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
                // ȭ�鿡�� ������� �ϴ� ������
                item.transform.DOScale(0f, animationDuration).SetEase(animationEase);
                item.GetComponent<CanvasGroup>().DOFade(0f, animationDuration).SetEase(animationEase)
                    .OnComplete(() => item.SetActive(false));
            }
            else
            {
                // ȭ�鿡 ��Ÿ���� �ϴ� ������
                item.SetActive(true);

                Vector3 targetPos = GetTargetPosition(displayIndex);
                float targetScale = GetTargetScale(displayIndex);
                float targetAlpha = GetTargetAlpha(displayIndex);

                // DOTween �ִϸ��̼�
                item.transform.DOMove(targetPos, animationDuration).SetEase(animationEase);
                item.transform.DOScale(targetScale, animationDuration).SetEase(animationEase);
                item.GetComponent<CanvasGroup>().DOFade(targetAlpha, animationDuration).SetEase(animationEase);
            }
        }

        // �ִϸ��̼� �Ϸ� ���
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
                // ȭ�鿡 ǥ�õ��� �ʴ� ������
                item.SetActive(false);
            }
            else
            {
                // ȭ�鿡 ǥ�õǴ� ������
                item.SetActive(true);
                item.transform.position = GetTargetPosition(displayIndex);
                item.transform.localScale = Vector3.one * GetTargetScale(displayIndex);
                item.GetComponent<CanvasGroup>().alpha = GetTargetAlpha(displayIndex);
            }
        }
    }

    // �������� ȭ���� ��� ��ġ�� �־�� �ϴ��� ��� (-1�̸� ȭ�鿡 ����)
    int GetDisplayIndex(int itemIndex)
    {
        if (allItems.Count <= 3)
        {
            // 3�� ���ϸ� ��� ǥ��
            return itemIndex;
        }

        // ���� �߾� �������� �������� ����� ��ġ ���
        int relativeIndex = (itemIndex - currentCenterIndex + allItems.Count) % allItems.Count;

        switch (relativeIndex)
        {
            case 0: return 0; // �߾�
            case 1: return 1; // ����
            case 2: return -1; // ����
            default:
                if (relativeIndex == allItems.Count - 1)
                    return 2; // ����
                else
                    return -1; // ����
        }
    }

    Vector3 GetTargetPosition(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0: return centerPos; // �߾�
            case 1: return rightPos;  // ����
            case 2: return leftPos;   // ����
            default: return centerPos;
        }
    }

    float GetTargetScale(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0: return centerScale; // �߾�
            default: return sideScale;  // �¿�
        }
    }

    float GetTargetAlpha(int displayIndex)
    {
        switch (displayIndex)
        {
            case 0: return centerAlpha; // �߾�
            default: return sideAlpha;  // �¿�
        }
    }

    // Ư�� �������� �߾����� ����
    public void SelectItem(int itemIndex)
    {
        if (isAnimating || itemIndex < 0 || itemIndex >= allItems.Count) return;

        currentCenterIndex = itemIndex;
        AnimateItems();
    }

    // ���� ���õ� ������ ������ ��ȯ
    public StageData GetCurrentStageData()
    {
        if (currentCenterIndex >= 0 && currentCenterIndex < allStageData.Count)
        {
            return allStageData[currentCenterIndex];
        }
        return null;
    }

    // ���� ���õ� ������ GameObject ��ȯ
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

    // ��� �������� �ִϸ��̼� ����
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