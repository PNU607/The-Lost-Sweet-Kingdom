using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRangeTower : RangeTower
{
    [SerializeField]
    private float attackDelay = 0.5f; // 공격 딜레이 시간

    private float startAttackTime = 0; // 공격 시작 시간

    private void Awake()
    {
        startAttackTime = 0;
    }

    /// <summary>
    /// 타겟을 향해 발사체를 생성
    /// </summary>
    /// <returns></returns>
    protected override void AttackToTarget()
    {
        // 타겟이 없으면
        if (closestAttackTarget == null)
        {
            // 타겟 탐색 상태로 전환
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟과의 거리 계산
        float distance = Vector3.Distance(closestAttackTarget.transform.position, transform.position);

        // 타겟과의 거리가 공격 범위보다 멀리 있으면
        if (distance > applyLevelData.attackRange)
        {
            // 타겟 탐색 상태로 전환
            attackTargets = null;
            towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        if (startAttackTime == 0)
        {
            startAttackTime = Time.time;

            towerAnim.SetBool("isAttacking", true);
            // 공격
            StartCoroutine(SetAttackAnimation());

            attackTimer = 0;
            startAttackTime = 0;
        }
    }

    /// <summary>
    /// 발사체 생성 후 세팅
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
