using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotTower : PinpointTower
{
    protected TowerWeapon attackingTowerWeapon; // 발사된 무기를 저장

    public override void Attack()
    {
        base.Attack();

        attackingTowerWeapon.Setup(closestAttackTarget.transform, this); // 발사된 무기를 타겟에 맞춰 설정
    }

    protected override TowerWeapon SpawnWeapon(Vector3 spawnPos, Transform targetTransform)
    {
        if (attackingTowerWeapon != null)
        {
            ReleaseWeapon(attackingTowerWeapon); // 이전에 발사된 무기가 있다면 제거
        }

        // 머리 위 이펙트 위치
        Vector3 attachTargetOffset = this.gameObject.transform.position + attackHeadOffset;

        TowerWeapon weapon = base.SpawnWeapon(attachTargetOffset, targetTransform); // 기본 무기 생성 로직 호출
        weapon.transform.SetParent(this.gameObject.transform);
        weapon.transform.position = attachTargetOffset; // 무기의 위치를 타겟의 머리 위로 설정

        attackingTowerWeapon = weapon; // 발사된 무기를 리스트에 추가

        return weapon;
    }

    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        attackingTowerWeapon = null; // 발사된 무기 변수에서 제거
        base.ReleaseWeapon(weapon);
    }
}
