using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Letter : TowerWeapon
{
    public float duration = 0f; // DamageZone�� ���� �ð�
    public float tickTimer = 0f;

    public WeaponInLetter towerWeapon;

    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);
        duration = shotTower.applyLevelData.attackDuration; // ���� �ð� ����
    }

    protected override void Update()
    {
        base.Update();

        if (target == null
                || target != null && !target.gameObject.activeSelf)
        {
            // Ÿ���� ������ ���� �߻� ����
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
