using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinpointTower : OneTargetRangeTower
{
    protected GameObject attachTarget; // 무기를 부착할 대상 오브젝트

    public override void Setup(TowerData nextTowerData, int level = 1)
    {
        base.Setup(nextTowerData, level);
        attachTarget = this.gameObject;
    }

    public override void Attack()
    {
        base.Attack();

        AttachSetUpWeapon(attachTarget);
    }

    /// <summary>
    /// Bullet을 Object Pool에 반환
    /// </summary>
    /// <param name="weapon"></param>
    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        DettachWeapon(weapon); // 무기를 분리하여 독립적으로 존재하도록 함
        base.ReleaseWeapon(weapon);
    }

    protected virtual void AttachSetUpWeapon(GameObject attachTarget)
    {
        // 머리 위 이펙트 위치
        Vector3 attachTargetOffset = attachTarget.transform.position + attackHeadOffset;

        TowerWeapon weapon = weaponPool.Spawn(attachTargetOffset);
        weapon.Setup(attachTarget.transform, this);
        weapon.transform.SetParent(attachTarget.transform);
    }

    protected virtual void DettachWeapon(TowerWeapon weapon)
    {
        weapon.transform.SetParent(null); // 무기의 부모를 해제하여 독립적으로 존재하도록 함
        weapon.transform.localScale = Vector3.one; // 무기의 스케일을 초기화하여 크기 문제 방지
    }
}
