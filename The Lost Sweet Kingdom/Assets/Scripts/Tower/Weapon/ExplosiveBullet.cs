/* 
 * @file: ExplosiveBullet
 * @author: 서지혜
 * @date: 2025-03-15
 * @brief: GunTower에서 발사하는 폭발하는 탄 발사체 스크립트
 * @details:
 *  - 가장 가까운 적 방향으로 직선 이동하여 적과 충돌하면 주위 일정 범위까지의 적들에게 데미지를 입힘
 * @see: GunTower.cs
 * @history:
 *  - 2025-03-15: ExplosiveBullet 스크립트 최초 작성
 */

using UnityEngine;

/* 
 * @class: ExplosiveBullet
 * @author: 서지혜
 * @date: 2025-03-15
 * @brief: GunTower에서 발사하는 폭발하는 탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 일정 범위 내의 적들에게 데미지 입히는 기능
 * @history:
 *  - 2025-03-15: ExplosiveBullet 클래스 최초 작성
 */
public class ExplosiveBullet : Bullet
{
    /// <summary>
    /// 충돌 체크
    /// 충돌한 collision의 태그가 Enemy이면, 일정 범위의 적들에게 데미지를 주고 충돌체는 파괴
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Explode();
        ReleaseBullet();
    }

    /// <summary>
    /// 일정 범위의 적들에게 데미지를 줌
    /// </summary>
    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, shotTower.CurrentTowerData.attackWeaponRange, shotTower.enemyLayer);
        foreach (Collider2D col in colliders)
        {
            EnemyTest enemy = col.GetComponent<EnemyTest>();
            if (enemy != null)
            {
                enemy.TakeDamage(shotTower.CurrentTowerData.attackDamage);
            }
        }
    }
}
