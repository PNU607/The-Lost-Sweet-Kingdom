using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinpointTower : OneTargetRangeTower
{
    /// <summary>
    /// Bullet�� Object Pool�� ��ȯ
    /// </summary>
    /// <param name="weapon"></param>
    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        DettachWeapon(weapon); // ���⸦ �и��Ͽ� ���������� �����ϵ��� ��
        base.ReleaseWeapon(weapon);
    }

    protected virtual void DettachWeapon(TowerWeapon weapon)
    {
        weapon.transform.SetParent(null); // ������ �θ� �����Ͽ� ���������� �����ϵ��� ��
        weapon.transform.localScale = weaponScale; // ������ �������� �ʱ�ȭ�Ͽ� ũ�� ���� ����
    }

    protected override TowerWeapon SpawnWeapon(Vector3 spawnPos, Transform targetTransform)
    {
        // �Ӹ� �� ����Ʈ ��ġ
        Vector3 attachTargetOffset = targetTransform.position + attackHeadOffset;

        TowerWeapon weapon = base.SpawnWeapon(attachTargetOffset, targetTransform); // �⺻ ���� ���� ���� ȣ��
        weapon.transform.SetParent(targetTransform);
        weapon.transform.position = attachTargetOffset; // ������ ��ġ�� Ÿ���� �Ӹ� ���� ����
        
        return weapon;
    }
}
