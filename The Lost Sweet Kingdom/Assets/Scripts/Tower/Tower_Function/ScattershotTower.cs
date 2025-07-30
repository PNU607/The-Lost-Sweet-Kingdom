using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotTower : PinpointTower
{
    protected List<TowerWeapon> attackingTowerWeapons = new List<TowerWeapon>(); // �߻�� ������� �����ϴ� ����Ʈ

    public override void Attack()
    {
        base.Attack();

        for (int i = 0; i < attackingTowerWeapons.Count; i++)
        {
            attackingTowerWeapons[i].Setup(closestAttackTarget.transform, this); // �߻�� ������� Ÿ�ٿ� ���� ����
        }
    }

    protected override TowerWeapon SpawnWeapon(Vector3 spawnPos, Transform targetTransform)
    {
        // �Ӹ� �� ����Ʈ ��ġ
        Vector3 attachTargetOffset = this.gameObject.transform.position + attackHeadOffset;

        TowerWeapon weapon = base.SpawnWeapon(attachTargetOffset, targetTransform); // �⺻ ���� ���� ���� ȣ��
        weapon.transform.SetParent(this.gameObject.transform);
        weapon.transform.position = attachTargetOffset; // ������ ��ġ�� Ÿ���� �Ӹ� ���� ����

        attackingTowerWeapons.Add(weapon); // �߻�� ���⸦ ����Ʈ�� �߰�

        return weapon;
    }

    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        attackingTowerWeapons.Remove(weapon); // �߻�� ���� ����Ʈ���� ����
        base.ReleaseWeapon(weapon);
    }
}
