using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRangeTower : RangeTower
{
    [SerializeField]
    private float attackDelay = 0.5f; // ���� ������ �ð�

    private float startAttackTime = 0; // ���� ���� �ð�

    private void Awake()
    {
        startAttackTime = 0;
    }

    /// <summary>
    /// Ÿ���� ���� �߻�ü�� ����
    /// </summary>
    /// <returns></returns>
    protected override void AttackToTarget()
    {
        // Ÿ���� ������
        if (closestAttackTarget == null)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // Ÿ���� ��Ȱ��ȭ�Ǹ�
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // Ÿ�ٰ��� �Ÿ� ���
        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        // Ÿ�ٰ��� �Ÿ��� ���� �������� �ָ� ������
        if (distance > applyLevelData.attackRange)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            attackTargets = null;
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        if (startAttackTime == 0)
        {
            startAttackTime = Time.time;

            towerAnim.SetBool("isAttacking", true);
            // ����
            StartCoroutine(SetAttackAnimation());

            attackTimer = 0;
            startAttackTime = 0;
        }
    }

    /// <summary>
    /// �߻�ü ���� �� ����
    /// </summary>
    private IEnumerator SetAttackAnimation()
    {
        while (Time.time - startAttackTime < applyLevelData.attackDuration)
        {
            Attack();

            yield return new WaitForSeconds(attackDelay);

            yield return null;
        }
    }
}
