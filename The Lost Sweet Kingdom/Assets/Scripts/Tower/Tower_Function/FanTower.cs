using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FanTower : TrackingTower
{
    public float angle = 60f;

    [SerializeField]
    private SpriteRenderer _effectRenderer;
    private MaterialPropertyBlock _mpb;

    private bool isAttacking = false; // ���� �� ����

    protected virtual void Awake()
    {
        _mpb = new MaterialPropertyBlock();
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
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // Ÿ���� ��Ȱ��ȭ�Ǹ�
        if (!closestAttackTarget.gameObject.activeSelf)
        {
            // Ÿ�� Ž�� ���·� ��ȯ
            towerBase.towerAnim.SetBool("isAttacking", false);
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
            towerBase.towerAnim.SetBool("isAttacking", false);
            ChangeState(TowerState.SearchTarget);
            return;
        }

        // ����
        SetAttackAnimation();
        attackTimer = 0;
    }

    /// <summary>
    /// �߻�ü ���� �� ����
    /// </summary>
    private void SetAttackAnimation()
    {
        towerBase.towerAnim.SetBool("isAttacking", true);
    }

    public override void Attack()
    {
        if (isAttacking)
        {
            return; // �̹� ���� ���̸� �ߺ� ���� ����
        }
        isAttacking = true; // ���� ����

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

        isAttacking = false; // ���� ����
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
