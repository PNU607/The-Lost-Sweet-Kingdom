using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;   // DOTween

public class StageSelectManager : MonoBehaviour
{
    enum SlotType
    {
        None,
        LessThanThree,
        LessThanFive,
        MoreThanFive
    }

    public List<StageData> stageDatas;           // �������� ������ ���
    public StageSlot slotPrefab;                 // ���� ������

    public List<Transform> objects;         // ȸ���� ������Ʈ�� (5��)
    public List<Transform> slots;           // ��ġ ���Ե� (5��, ���� �̸� ��ġ)
    public float jumpPower = 2f;             // ������ ����
    public float duration = 0.5f;             // �̵� �ð�
    public Vector3 centerScale = Vector3.one * 1.6f; // ��� Ȯ�� ����
    public Vector3 normalScale = Vector3.one;        // �Ϲ� ũ��

    private int centerIndex = 2; // ��� ���� �ε��� (0~4 �� 2�� �߾�)

    private SlotType currentSlotType = SlotType.None;

    void Start()
    {
        if (stageDatas.Count == 0) return;

        if (stageDatas.Count < 3)
        {
            currentSlotType = SlotType.LessThanThree;

            for (int i = 0; i < stageDatas.Count; i++)
            {
                var slot = Instantiate(slotPrefab, slots[i + 2]);
                slot.SetData(stageDatas[i]);
                objects.Add(slot.transform);
            }
        }
        else if (stageDatas.Count < slots.Count)
        {
            currentSlotType = SlotType.LessThanFive;

            for (int i = 0; i < stageDatas.Count; i++)
            {
                var slot = Instantiate(slotPrefab, slots[i + 1]);
                slot.SetData(stageDatas[i]);
                objects.Add(slot.transform);
            }

            if (objects.Count == 3)
            {
                var slot = Instantiate(slotPrefab, slots[0]);
                slot.SetData(stageDatas[2]);
                objects.Insert(0, slot.transform);

                slot = Instantiate(slotPrefab, slots[4]);
                slot.SetData(stageDatas[0]);
                objects.Add(slot.transform);
            }
            else if (objects.Count == 4)
            {
                var slot = Instantiate(slotPrefab, slots[0]);
                slot.SetData(stageDatas[3]);
                objects.Insert(0, slot.transform);
            }
        }
        else
        {
            currentSlotType = SlotType.MoreThanFive;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = Instantiate(slotPrefab, slots[i]);
                slot.SetData(stageDatas[i]);
                objects.Add(slot.transform);
            }
        }
    }

    public void MoveLeft()
    {
        // ����Ʈ ȸ�� (�� ���� �� �ڷ�)
        Transform first = objects[0];
        objects.RemoveAt(0);
        objects.Add(first);

        AnimateAll();
    }

    public void MoveRight()
    {
        // ����Ʈ ȸ�� (�� �ڸ� �� ������)
        Transform last = objects[^1];
        objects.RemoveAt(objects.Count - 1);
        objects.Insert(0, last);

        AnimateAll();
    }

    private void AnimateAll()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            Transform obj = objects[i];
            Transform slot = slots[i];

            obj.DOJump(slot.position, jumpPower, 1, duration).SetEase(Ease.OutQuad);

            if (i == centerIndex)
                obj.DOScale(centerScale, duration * 0.8f);
            else
                obj.DOScale(normalScale, duration * 0.8f);
        }
    }
}
