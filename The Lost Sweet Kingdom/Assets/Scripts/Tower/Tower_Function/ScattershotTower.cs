using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScattershotTower : PinpointTower
{
    public override void Attack()
    {
        base.Attack();

        if (candidates.Count > 0)
        {
            EnemyTest closest = null;
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
                attachTarget = closest.gameObject;
            }
        }
    }
}
