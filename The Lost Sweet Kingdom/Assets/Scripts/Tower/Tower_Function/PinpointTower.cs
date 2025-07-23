using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinpointTower : OneTargetRangeTower
{
    protected GameObject attachTarget; // ���⸦ ������ ��� ������Ʈ

    public override void Attack()
    {
        base.Attack();

        if (candidates.Count > 0)
        {
            EnemyTest closest = null;
            float minDist = float.MaxValue;
            Vector3 towerPos = transform.position;

            foreach (var enemy in candidates)
            {
                float dist = Vector3.SqrMagnitude(towerPos - enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }

            if (closest != null)
            {
                //AttachSetUpWeapon(closest.gameObject);
                AttachSetUpWeapon(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Bullet�� Object Pool�� ��ȯ
    /// </summary>
    /// <param name="weapon"></param>
    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        DettachWeapon(weapon); // ���⸦ �и��Ͽ� ���������� �����ϵ��� ��
        base.ReleaseWeapon(weapon);
    }

    protected virtual void AttachSetUpWeapon(GameObject attachTarget)
    {
        // �Ӹ� �� ����Ʈ ��ġ
        Vector3 attachTargetOffset = attachTarget.transform.position + attackHeadOffset;

        TowerWeapon weapon = weaponPool.Spawn(attachTargetOffset);
        weapon.Setup(attachTarget.transform, this);
        weapon.transform.SetParent(attachTarget.transform);
    }

    protected virtual void DettachWeapon(TowerWeapon weapon)
    {
        weapon.transform.SetParent(null); // ������ �θ� �����Ͽ� ���������� �����ϵ��� ��
        weapon.transform.localScale = Vector3.one; // ������ �������� �ʱ�ȭ�Ͽ� ũ�� ���� ����
    }
}
