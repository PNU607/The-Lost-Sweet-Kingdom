using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotTower : PinpointTower
{
    protected List<TowerWeapon> attackingTowerWeapons = new List<TowerWeapon>(); // 발사된 무기들을 저장하는 리스트

    public override void Attack()
    {
        base.Attack();

        for (int i = 0; i < attackingTowerWeapons.Count; i++)
        {
            attackingTowerWeapons[i].Setup(closestAttackTarget.transform, this); // 발사된 무기들을 타겟에 맞춰 설정
        }
    }

    protected override TowerWeapon SpawnWeapon(Vector3 spawnPos, Transform targetTransform)
    {
        // 머리 위 이펙트 위치
        Vector3 attachTargetOffset = this.gameObject.transform.position + attackHeadOffset;

        TowerWeapon weapon = base.SpawnWeapon(attachTargetOffset, targetTransform); // 기본 무기 생성 로직 호출
        weapon.transform.SetParent(this.gameObject.transform);
        weapon.transform.position = attachTargetOffset; // 무기의 위치를 타겟의 머리 위로 설정

        attackingTowerWeapons.Add(weapon); // 발사된 무기를 리스트에 추가

        return weapon;
    }

    public override void ReleaseWeapon(TowerWeapon weapon)
    {
        attackingTowerWeapons.Remove(weapon); // 발사된 무기 리스트에서 제거
        base.ReleaseWeapon(weapon);
    }
}
