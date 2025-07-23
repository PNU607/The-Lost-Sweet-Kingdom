using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInLetter : Missile
{
    protected override void ReleaseWeapon()
    {
        isSetup = false;

        if (shotTower != null)
        {
            //shotTower.GetComponent<Tower>().ReleaseWeapon(this);
            Destroy(gameObject); // Object Pool을 사용하지 않는 경우, 게임 오브젝트를 파괴
        }
        else
        {
            Debug.Log("shotTower 없음");
        }
    }
}
