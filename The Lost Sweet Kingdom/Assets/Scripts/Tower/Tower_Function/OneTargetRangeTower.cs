using System.Collections.Generic;
using UnityEngine;

public class OneTargetRangeTower : RangeTower
{
    protected Vector3 weaponScale = Vector3.one; // ������ �⺻ ������

    public override void Attack()
    {
        base.Attack();

        if (candidates.Count > 0)
        {
            Enemy closest = null;
            float minDist = float.MaxValue;
            Vector3 towerPos = transform.position;

            foreach (var enemy in candidates)
            {
                float dist = Vector3.SqrMagnitude(towerPos - enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }

            if (closest != null)
            {
                // �Ӹ� �� ����Ʈ ��ġ
                Vector3 effectPos = closest.transform.position + attackHeadOffset;

                TowerWeapon weapon = SpawnWeapon(effectPos, closest.transform);
                weaponScale = weapon.transform.localScale; // ������ �⺻ �������� ����


                //var health = closest.GetComponent<EnemyTest>();

                //closest.TakeDamage(applyLevelData.attackDamage);
            }
        }
    }

    protected override void AttackRangeTarget(Collider2D enemyCol)
    {
        return; // ���� Ÿ�� �����̹Ƿ� ���� ���� ������ ������� ����
    }
}
