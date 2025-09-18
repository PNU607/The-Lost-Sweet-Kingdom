using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Transform slotParent;
    [SerializeField] private StageSlot slotPrefab;

    [Header("Stage Configuration")]
    [SerializeField] private List<StageData> stageDatas;
    [SerializeField] private List<Transform> positionSlots; // 5���� ��ġ ����

    [Header("Animation Settings")]
    [SerializeField] private float baseDuration = 0.5f;
    [SerializeField] private float minDuration = 0.3f;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float centerScaleMultiplier = 1.6f;
    [SerializeField] private float inputCooldownThreshold = 0.3f;
    [SerializeField] private float durationDecreaseRate = 0.8f;

    // Constants
    private const int CENTER_INDEX = 2;
    private const int MAX_SLOTS = 5;

    // Runtime variables
    private readonly List<StageSlot> activeSlots = new List<StageSlot>();
    private readonly List<int> currentDataIndices = new List<int>(); // ���� �� ������ �����ϴ� ������ �ε���
    private Vector3 centerScale;
    private Vector3 normalScale = Vector3.one;

    private float currentDuration;
    private float lastInputTime;
    private bool isAnimating;

    // Object pooling for efficiency
    private Queue<StageSlot> slotPool = new Queue<StageSlot>();

    #region Unity Lifecycle

    private void Awake()
    {
        centerScale = Vector3.one * centerScaleMultiplier;
        currentDuration = baseDuration;
    }

    private void Start()
    {
        InitializeSlots();
    }

    private void OnDestroy()
    {
        // Clean up DOTween sequences
        DOTween.KillAll();
    }

    #endregion

    #region Initialization

    private void InitializeSlots()
    {
        if (stageDatas == null || stageDatas.Count == 0)
        {
            Debug.LogWarning("No stage data available!");
            return;
        }

        if (positionSlots == null || positionSlots.Count != MAX_SLOTS)
        {
            Debug.LogError($"Position slots must have exactly {MAX_SLOTS} elements!");
            return;
        }

        CreateInitialSlots();
        UpdateCenterSlot();
        SetInitialOverlays();
    }

    private void CreateInitialSlots()
    {
        activeSlots.Clear();
        currentDataIndices.Clear();

        int dataCount = stageDatas.Count;

        for (int i = 0; i < MAX_SLOTS; i++)
        {
            var slot = GetOrCreateSlot();
            slot.transform.position = positionSlots[i].position;

            // ������ �ε��� ���
            int dataIndex;
            if (dataCount >= MAX_SLOTS)
            {
                // �����Ͱ� ����� ��� ���� ����
                dataIndex = i;
            }
            else
            {
                // �����Ͱ� ���� ��� ��ȯ ��ġ
                if (dataCount == 1)
                {
                    dataIndex = i == CENTER_INDEX ? 0 : -1;
                }
                else if (dataCount == 2)
                {
                    if (i == 1) dataIndex = 0;
                    else if (i == 3) dataIndex = 1;
                    else dataIndex = -1;
                }
                else if (dataCount == 3)
                {
                    // 3���� ��� 1, 2, 0, 1, 2 �������� ����
                    int[] pattern = { 1, 2, 0, 1, 2 };
                    dataIndex = pattern[i];
                }
                else
                {
                    // 4�� �̻��� ��� �⺻ ��ȯ
                    dataIndex = (i + 1) % dataCount;
                }
            }

            if (dataIndex >= 0 && dataIndex < dataCount)
            {
                slot.SetData(stageDatas[dataIndex]);
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }

            activeSlots.Add(slot);
            currentDataIndices.Add(dataIndex);
        }
    }

    private void SetInitialOverlays()
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            if (activeSlots[i] != null && activeSlots[i].gameObject.activeSelf)
            {
                if (i == CENTER_INDEX)
                {
                    activeSlots[i].OverlayOff();
                    activeSlots[i].transform.localScale = centerScale;
                }
                else
                {
                    activeSlots[i].OverlayOn();
                    activeSlots[i].transform.localScale = normalScale;
                }
            }
        }
    }

    #endregion

    #region Movement

    public void MoveLeft()
    {
        if (isAnimating || stageDatas.Count <= 1) return;

        HandleInput();
        ShiftSlotsLeft();
        AnimateSlots();
    }

    public void MoveRight()
    {
        if (isAnimating || stageDatas.Count <= 1) return;

        HandleInput();
        ShiftSlotsRight();
        AnimateSlots();
    }

    private void ShiftSlotsLeft()
    {
        // ���Ե��� �������� ��ȯ �̵� (ù ��° ������ ����������)
        if (activeSlots.Count > 0)
        {
            var firstSlot = activeSlots[0];
            activeSlots.RemoveAt(0);
            activeSlots.Add(firstSlot);
        }

        // ������ �ε����� �Բ� ��ȯ �̵�
        ShiftDataIndicesLeft();
        UpdateSlotData();
    }

    private void ShiftSlotsRight()
    {
        // ���Ե��� ���������� ��ȯ �̵� (������ ������ ù ��°��)
        if (activeSlots.Count > 0)
        {
            var lastSlot = activeSlots[activeSlots.Count - 1];
            activeSlots.RemoveAt(activeSlots.Count - 1);
            activeSlots.Insert(0, lastSlot);
        }

        // ������ �ε����� �Բ� ��ȯ �̵�
        ShiftDataIndicesRight();
        UpdateSlotData();
    }

    private void ShiftDataIndicesLeft()
    {
        // �����Ͱ� ���� ���� ������ �����ϸ鼭 ��ȯ
        int dataCount = stageDatas.Count;

        for (int i = 0; i < currentDataIndices.Count; i++)
        {
            if (currentDataIndices[i] >= 0)
            {
                currentDataIndices[i] = (currentDataIndices[i] + 1) % dataCount;
            }
        }
    }

    private void ShiftDataIndicesRight()
    {
        // �����Ͱ� ���� ���� ������ �����ϸ鼭 ��ȯ
        int dataCount = stageDatas.Count;

        for (int i = 0; i < currentDataIndices.Count; i++)
        {
            if (currentDataIndices[i] >= 0)
            {
                currentDataIndices[i] = (currentDataIndices[i] - 1 + dataCount) % dataCount;
            }
        }
    }

    private void UpdateSlotData()
    {
        // ��� Ȱ�� ������ �����͸� ���� �ε����� �°� ������Ʈ
        for (int i = 0; i < activeSlots.Count; i++)
        {
            if (activeSlots[i] != null && activeSlots[i].gameObject.activeSelf &&
                currentDataIndices[i] >= 0 && currentDataIndices[i] < stageDatas.Count)
            {
                activeSlots[i].SetData(stageDatas[currentDataIndices[i]]);
            }
        }
    }

    #endregion

    #region Input & Animation

    private void HandleInput()
    {
        float now = Time.time;
        float interval = now - lastInputTime;
        lastInputTime = now;

        // Accelerate animation on rapid input
        currentDuration = interval < inputCooldownThreshold
            ? Mathf.Max(minDuration, currentDuration * durationDecreaseRate)
            : baseDuration;

        // Kill existing animations
        KillAllAnimations();
    }

    private void AnimateSlots()
    {
        isAnimating = true;
        int completedAnimations = 0;
        int activeSlotCount = GetActiveSlotCount();

        for (int i = 0; i < activeSlots.Count; i++)
        {
            if (activeSlots[i] == null || !activeSlots[i].gameObject.activeSelf) continue;

            Transform slotTransform = activeSlots[i].transform;
            Vector3 targetPosition = positionSlots[i].position;
            bool isCenter = (i == CENTER_INDEX);

            // ������ ���� �ִϸ��̼����� ��ġ �̵�
            slotTransform.DOJump(targetPosition, jumpPower, 1, currentDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    completedAnimations++;
                    if (completedAnimations >= activeSlotCount)
                    {
                        isAnimating = false;
                        UpdateCenterSlot();
                    }
                });

            // ������ �ִϸ��̼�
            Vector3 targetScale = isCenter ? centerScale : normalScale;
            slotTransform.DOScale(targetScale, currentDuration * 0.8f)
                .SetEase(Ease.OutQuad);

            // �������� ���� ������Ʈ
            UpdateSlotOverlay(i, isCenter);
        }
    }

    private void UpdateSlotOverlay(int index, bool isCenter)
    {
        if (activeSlots[index] == null || !activeSlots[index].gameObject.activeSelf) return;

        if (isCenter)
        {
            activeSlots[index].OverlayOff();
        }
        else
        {
            activeSlots[index].OverlayOn();
        }
    }

    private int GetActiveSlotCount()
    {
        int count = 0;
        foreach (var slot in activeSlots)
        {
            if (slot != null && slot.gameObject.activeSelf)
                count++;
        }
        return count;
    }

    private void KillAllAnimations()
    {
        foreach (var slot in activeSlots)
        {
            if (slot != null)
                slot.transform.DOKill();
        }
    }

    private void UpdateCenterSlot()
    {
        if (activeSlots.Count > CENTER_INDEX && activeSlots[CENTER_INDEX] != null &&
            activeSlots[CENTER_INDEX].gameObject.activeSelf)
        {
            var centerSlot = activeSlots[CENTER_INDEX];
            stageText.text = centerSlot.GetStageName();
            centerSlot.transform.localScale = centerScale;
            centerSlot.OverlayOff();
        }
    }

    #endregion

    #region Object Pooling

    private StageSlot GetOrCreateSlot()
    {
        if (slotPool.Count > 0)
        {
            var pooledSlot = slotPool.Dequeue();
            pooledSlot.gameObject.SetActive(true);
            return pooledSlot;
        }

        return Instantiate(slotPrefab, slotParent);
    }

    private void ReturnToPool(StageSlot slot)
    {
        slot.gameObject.SetActive(false);
        slotPool.Enqueue(slot);
    }

    #endregion

    #region Public Methods

    public void RefreshStageData(List<StageData> newStageDatas)
    {
        // Clean up existing slots
        foreach (var slot in activeSlots)
        {
            ReturnToPool(slot);
        }
        activeSlots.Clear();
        currentDataIndices.Clear();

        // Update data and reinitialize
        stageDatas = newStageDatas;
        InitializeSlots();
    }

    public StageData GetCurrentStageData()
    {
        if (activeSlots.Count > CENTER_INDEX && activeSlots[CENTER_INDEX] != null &&
            activeSlots[CENTER_INDEX].gameObject.activeSelf)
        {
            return activeSlots[CENTER_INDEX].GetStageData();
        }
        return null;
    }

    #endregion
}