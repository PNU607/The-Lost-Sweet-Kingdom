/* 
 * @file: SlowMissile.cs
 * @author: 서지혜
 * @date: 2025-03-14
 * @brief: GunTower에서 발사하는 적 이속을 느리게 하는 유도탄 발사체 스크립트
 * @details:
 *  - Missile에서 적 이속을 느리게 하는 디버프 기능이 추가된 버전
 * @see: GunTower.cs, Missile.cs
 * @history:
 *  - 2025-03-14: SlowMissile 스크립트 최초 작성
 */

using System.Collections;
using UnityEngine;

/* 
 * @class: SlowMissile
 * @author: 서지혜
 * @date: 2025-03-14
 * @brief: GunTower에서 발사하는 적 이속을 느리게 하는 유도탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 적의 이속을 줄이는 기능
 * @history:
 *  - 2025-03-14: SlowMissile 클래스 최초 작성
 */
public class SlowMissile : Missile
{
    /// <summary>
    /// 발사체의 공격 기능
    /// </summary>
    /// <param name="collision"></param>
    protected override void Attack(Collider2D collision)
    {
        EnemyTest enemy = collision.GetComponent<EnemyTest>();

        StartCoroutine(SlowEnemy(enemy));
    }

    /// <summary>
    /// 일정시간동안 적의 이속을 느리게 만듦
    /// </summary>
    /// <param name="enemy">공격할 적</param>
    /// <returns></returns>
    private IEnumerator SlowEnemy(EnemyTest enemy)
    {
        if (enemy == null) yield break;

        // 적 이속 감소
        TowerData towerData = shotTower.CurrentTowerData;
        enemy.SetSpeedMultiplier(1 / towerData.levelDatas[shotTower.towerLevel].attackDamage, towerData.levelDatas[shotTower.towerLevel].attackDuration);
    }
}
