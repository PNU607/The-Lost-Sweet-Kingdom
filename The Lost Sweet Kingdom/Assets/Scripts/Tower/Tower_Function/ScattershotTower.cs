using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotTower : PinpointTower
{
    protected TowerWeapon attackingTowerWeapon; // �߻�� ���⸦ ����

    public override void Attack()
    {
        base.Attack();

        attackingTowerWeapon.Setup(closestAttackTarget.transform, this); // �߻�� ���⸦ Ÿ�ٿ� ���� ����
    }

    protected override TowerWeapon SpawnWeapon(Vector3 spawnPos, Transform targetTransform)
    {
        if (attackingTowerWeapon != null)
        {
            ReleaseWeapon(attackingTowerWeapon); // ������ �߻�� ���Ⱑ �ִٸ� ����
        }

        // �Ӹ� �� ����Ʈ ��ġ
        Vector3 attachTargetOffset = this.gameObject.transform.position + attackHeadOffset;

        TowerWeapon weapon = base.SpawnWeapon(attachTargetOffset, targetTransform); // �⺻ ���� ���� ���� ȣ��
        weapon.transform.SetParent(this.gameObject.transform);
        weapon.transform.position = attachTargetOffset; // ������ ��ġ�� Ÿ���� �Ӹ� ���� ����

        attackingTowerWeapon = weapon; // �߻�� ���⸦ ����Ʈ�� �߰�

        return weapon;
    }

    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        attackingTowerWeapon = null; // �߻�� ���� �������� ����
        base.ReleaseWeapon(weapon);
    }
}
