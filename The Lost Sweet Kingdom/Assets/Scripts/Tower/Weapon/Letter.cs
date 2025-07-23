using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Letter : TowerWeapon
{
    public float duration = 0f; // DamageZone의 지속 시간
    public float tickTimer = 0f;

    public WeaponInLetter towerWeapon;

    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);
        duration = shotTower.applyLevelData.attackDuration; // 지속 시간 설정
    }

    protected override void Update()
    {
        base.Update();

        if (target == null
                || target != null && !target.gameObject.activeSelf)
        {
            // 타겟이 없으면 무기 발사 중지
            isAttackStopped = true;
            ReleaseWeapon();
            return;
        }

        duration -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= .3f)
        {
            tickTimer = 0f;

            if (target != null)
            {
                WeaponInLetter clone = Instantiate(towerWeapon);
                clone.transform.SetPositionAndRotation(this.transform.position, default);
                clone.gameObject.SetActive(true);

                clone.Setup(target.transform, shotTower);
            }
        }

        if (duration <= 0f)
        {
            //Debug.Log(duration);
            ReleaseWeapon();
        }
    }
}
