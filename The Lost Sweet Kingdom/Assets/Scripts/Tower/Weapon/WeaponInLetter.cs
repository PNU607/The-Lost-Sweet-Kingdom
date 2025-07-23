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
            Destroy(gameObject); // Object Pool�� ������� �ʴ� ���, ���� ������Ʈ�� �ı�
        }
        else
        {
            Debug.Log("shotTower ����");
        }
    }
}
