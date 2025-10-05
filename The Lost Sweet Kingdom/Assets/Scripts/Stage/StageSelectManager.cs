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
    [SerializeField] private List<Transform> positionSlots; // 5개의 위치 슬롯

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
    private readonly List<int> currentDataIndices = new List<int>(); // 현재 각 슬롯이 참조하는 데이터 인덱스
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

            // 데이터 인덱스 계산
            int dataIndex;
            if (dataCount >= MAX_SLOTS)
            {
                // 데이터가 충분한 경우 직접 매핑
                dataIndex = i;
            }
            else
            {
                // 데이터가 적은 경우 순환 배치
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
                    // 3개인 경우 1, 2, 0, 1, 2 패턴으로 시작
                    int[] pattern = { 1, 2, 0, 1, 2 };
                    dataIndex = pattern[i];
                }
                else
                {
                    // 4개 이상인 경우 기본 순환
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
    public void Moveleft()
    {
        if (isAnimating || stageDatas.Count <= 1) return;

        HandleInput();
        ShiftSlotsRight();
        AnimateSlots();
    }

    public void MoveRight()
    {
        if (isAnimating || stageDatas.Count <= 1) return;

        HandleInput();
        ShiftSlotsLeft();
        AnimateSlots();
    }

    private void ShiftSlotsLeft()
    {
        // 슬롯들을 왼쪽으로 순환 이동 (첫 번째 슬롯이 마지막으로)
        if (activeSlots.Count > 0)
        {
            var firstSlot = activeSlots[0];
            activeSlots.RemoveAt(0);
            activeSlots.Add(firstSlot);
        }

        // 데이터 인덱스도 함께 순환 이동
        ShiftDataIndicesLeft();
        UpdateSlotData();
    }

    private void ShiftSlotsRight()
    {
        // 슬롯들을 오른쪽으로 순환 이동 (마지막 슬롯이 첫 번째로)
        if (activeSlots.Count > 0)
        {
            var lastSlot = activeSlots[activeSlots.Count - 1];
            activeSlots.RemoveAt(activeSlots.Count - 1);
            activeSlots.Insert(0, lastSlot);
        }

        // 데이터 인덱스도 함께 순환 이동
        ShiftDataIndicesRight();
        UpdateSlotData();
    }

    private void ShiftDataIndicesLeft()
    {
        // 데이터가 적을 때는 패턴을 유지하면서 순환
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
        // 데이터가 적을 때는 패턴을 유지하면서 순환
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
        // 모든 활성 슬롯의 데이터를 현재 인덱스에 맞게 업데이트
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

            // 간단한 점프 애니메이션으로 위치 이동
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

            // 스케일 애니메이션
            Vector3 targetScale = isCenter ? centerScale : normalScale;
            slotTransform.DOScale(targetScale, currentDuration * 0.8f)
                .SetEase(Ease.OutQuad);

            // 오버레이 상태 업데이트
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

    void Update()
    {
        // 키보드 입력 처리
        if (Input.GetKeyDown(KeyCode.LeftArrow) /*|| Input.GetKeyDown(KeyCode.A)*/)
        {
            // 왼쪽으로 회전
            Moveleft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) /*|| Input.GetKeyDown(KeyCode.D)*/)
        {
            // 오른쪽으로 회전
            MoveRight();
        }

        // 선택 확인
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 게임 시작
            activeSlots[CENTER_INDEX].LoadStageScene();
        }
    }
    #endregion
}