using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinpointTower : OneTargetRangeTower
{
    /// <summary>
    /// Bullet을 Object Pool에 반환
    /// </summary>
    /// <param name="weapon"></param>
    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        DettachWeapon(weapon); // 무기를 분리하여 독립적으로 존재하도록 함
        base.ReleaseWeapon(weapon);
    }

    protected virtual void DettachWeapon(TowerWeapon weapon)
    {
        weapon.transform.SetParent(null); // 무기의 부모를 해제하여 독립적으로 존재하도록 함
        weapon.transform.localScale = weaponScale; // 무기의 스케일을 초기화하여 크기 문제 방지
    }

    protected override TowerWeapon SpawnWeapon(Vector3 spawnPos, Transform targetTransform)
    {
        // 머리 위 이펙트 위치
        Vector3 attachTargetOffset = targetTransform.position + attackHeadOffset;

        TowerWeapon weapon = base.SpawnWeapon(attachTargetOffset, targetTransform); // 기본 무기 생성 로직 호출
        weapon.transform.SetParent(targetTransform);
        weapon.transform.position = attachTargetOffset; // 무기의 위치를 타겟의 머리 위로 설정
        
        return weapon;
    }
}
