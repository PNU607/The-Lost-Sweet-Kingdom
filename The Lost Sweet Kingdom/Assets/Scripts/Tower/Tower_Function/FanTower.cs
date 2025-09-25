using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FanTower : TrackingTower
{
    public float angle = 60f;

    [SerializeField]
    private SpriteRenderer _effectRenderer;
    private MaterialPropertyBlock _mpb;

    private bool isAttacking = false; // 공격 중 여부

    protected virtual void Awake()
    {
        _mpb = new MaterialPropertyBlock();
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
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 타겟이 비활성화되면
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // 타겟 탐색 상태로 전환
            towerBase.towerAnim.SetBool("isAttacking", false);
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
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // 공격
        SetAttackAnimation();
        attackTimer = 0;
    }

    /// <summary>
    /// 발사체 생성 후 세팅
    /// </summary>
    private void SetAttackAnimation()
    {
        towerBase.towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        if (isAttacking)
        {
            return; // 이미 공격 중이면 중복 공격 방지
        }
        isAttacking = true; // 공격 시작

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, applyLevelData.attackRange, towerBase.enemyLayer);

        Vector2 forward = transform.right;
        if (isFlipX)
        {
            _effectRenderer.flipX = false;
            forward = -forward;
        }
        else
        {
            _effectRenderer.flipX = true;
        }
        StartCoroutine(PlayShockwave());

        foreach (var hit in hits)
        {
            Vector2 toTarget = ((Vector2)hit.transform.position - (Vector2)transform.position);
            float distance = toTarget.magnitude;

            float currentAngle = Vector2.Angle(forward, toTarget);
            if (currentAngle - 150f <= angle / 2f)
            {
                float damage = applyLevelData.attackDamage;

                hit.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        isAttacking = false; // 공격 종료
    }

    IEnumerator PlayShockwave()
    {
        _effectRenderer.transform.localScale = new Vector3(applyLevelData.attackRange * 2, applyLevelData.attackRange * 2, 1);

        float duration = 0.5f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = Mathf.Clamp01(t / duration);
            _effectRenderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_Progress", progress);
            _effectRenderer.SetPropertyBlock(_mpb);
            yield return null;
        }

        _effectRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Progress", 1f);
        _effectRenderer.SetPropertyBlock(_mpb);
        yield return new WaitForSeconds(.1f);

        _effectRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_Progress", 0f);
        _effectRenderer.SetPropertyBlock(_mpb);
    }
}
