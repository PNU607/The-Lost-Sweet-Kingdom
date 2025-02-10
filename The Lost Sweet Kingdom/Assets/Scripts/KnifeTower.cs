/* 
 * @file: KnifeTower.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 계속 회전하며 무기와 맞은 적에게 대미지를 입히는 타워 스크립트
 * @details:
 *  - 무기와 충돌하고 있는 적들에게 지속적으로 공격하는 기능
 * @see: RotatingTower.cs, Tower.cs, Enemy.cs
 * @history:
 *  - 2025-02-09: KnifeTower 스크립트 최초 작성
 */

using System.Collections.Generic;
using UnityEngine;

/* 
 * @class: KnifeTower
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 계속 회전하며 무기와 맞은 적에게 대미지를 입히는 타워 클래스
 * @details:
 *  - 충돌 체크, 공격 기능
 * @history:
 *  - 2025-02-09: KnifeTower 클래스 최초 작성
 */
public class KnifeTower : RotatingTower
{
    /// <summary>
    /// 개별 적 공격 타이머
    /// </summary>
    private Dictionary<Enemy, float> lastAttackTime = new Dictionary<Enemy, float>(); // 개별 적 공격 타이머

    /// <summary>
    /// 무기가 적과 충돌하면 대미지 적용
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                TryDamageEnemy(enemy);
            }
        }
    }

    /// <summary>
    /// 적에게 대미지를 주되, 일정 시간 간격을 둠
    /// </summary>
    /// <param name="enemy"></param>
    private void TryDamageEnemy(Enemy enemy)
    {
        if (lastAttackTime.ContainsKey(enemy))
        {
            // 공격 쿨타임이 아직 지나지 않았으면
            if (Time.time - lastAttackTime[enemy] < currentTowerData.attackCooldown)
            {
                return;
            }

            // 대미지 적용
            enemy.TakeDamage(currentTowerData.attackDamage);
            // 마지막 공격 시간 기록
            lastAttackTime[enemy] = Time.time;
        }
        else
        {
            // 대미지 적용
            enemy.TakeDamage(currentTowerData.attackDamage);
            // 마지막 공격 시간 기록
            lastAttackTime.Add(enemy, Time.time);
        }
    }
}
