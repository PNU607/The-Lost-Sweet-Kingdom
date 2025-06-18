/* 
 * @file: Missile.cs
 * @author: 서지혜
 * @date: 2025-02-23
 * @brief: GunTower에서 발사하는 유도탄 발사체 스크립트
 * @details:
 *  - 적 방향으로 계속 이동하여 적과 충돌하면 대미지를 입힘
 * @see: GunTower.cs, Bullet.cs
 * @history:
 *  - 2025-02-23: Missile 스크립트 최초 작성
 *  - 2025-03-23: TowerWeapon 추가로 무기의 이동 기능 로직 수정
 */

using UnityEngine;
/* 
 * @class: Missile
 * @author: 서지혜
 * @date: 2025-02-23
 * @brief: GunTower에서 발사하는 유도탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 적의 hp를 줄이는 기능
 *  - Movement2D를 이용해 적 방향으로 발사체 이동 기능
 * @history:
 *  - 2025-02-23: Missile 클래스 최초 작성
 *  - 2025-03-23: Update 함수 삭제 및 MoveWeapon 추가
 */

public class Missile : Bullet
{
    /// <summary>
    /// 무기의 이동 방향 설정
    /// </summary>
    protected override void SetDirection()
    {
        base.SetDirection();

        direction = (target.position - transform.position).normalized;
    }

    /// <summary>
    /// 충돌 체크
    /// 충돌한 collision의 태그가 Enemy이면, 타겟에 데미지를 주고 충돌체는 파괴
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        if (collision.transform != target) return;
        if (!this.gameObject.activeSelf) return;
        Attack(collision);
        ReleaseWeapon();
    }
}
