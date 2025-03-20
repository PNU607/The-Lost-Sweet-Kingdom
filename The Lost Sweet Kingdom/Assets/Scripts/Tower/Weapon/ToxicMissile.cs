/* 
 * @file: ToxicMissile.cs
 * @author: 서지혜
 * @date: 2025-03-14
 * @brief: GunTower에서 발사하는 지속 데미지를 주는 유도탄 발사체 스크립트
 * @details:
 *  - 적 방향으로 계속 이동하여 적과 충돌하면 지속 데미지를 입힘
 * @see: GunTower.cs, Missile.cs
 * @history:
 *  - 2025-03-14: ToxicMissile 스크립트 최초 작성
 */

using System.Collections;
using UnityEngine;

/* 
 * @class: ToxicMissile
 * @author: 서지혜
 * @date: 2025-03-14
 * @brief: GunTower에서 발사하는 지속 데미지를 주는 유도탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 적의 hp를 일정 시간동안 지속적으로 줄이는 기능
 * @history:
 *  - 2025-03-14: ToxicMissile 클래스 최초 작성
 */
public class ToxicMissile : Missile
{
    /// <summary>
    /// 발사체의 공격 기능
    /// </summary>
    /// <param name="collision"></param>
    protected override void Attack(Collider2D collision)
    {
        EnemyTest enemy = collision.GetComponent<EnemyTest>();
        StartCoroutine(DealContinuousDamage(enemy));
    }

    /// <summary>
    /// 지속적으로 데미지를 입힘
    /// </summary>
    /// <param name="enemy">공격할 적</param>
    /// <returns></returns>
    private IEnumerator DealContinuousDamage(EnemyTest enemy)
    {
        if (enemy == null) yield break;

        // 적 이속 감소
        TowerData towerData = shotTower.CurrentTowerData;
        enemy.TakeContinuousDamageForBullet(towerData.attackDamage, towerData.attackDuration);
    }
}
