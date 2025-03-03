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
 */

using UnityEngine;

/* 
 * @class: Missile
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: GunTower에서 발사하는 유도탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 적의 hp를 줄이는 기능
 *  - Movement2D를 이용해 적 방향으로 발사체 이동 기능
 * @history:
 *  - 2025-02-23: Missile 클래스 최초 작성
 */
public class Missile : Bullet
{
    /// <summary>
    /// 업데이트
    /// 타겟이 있으면 발사체가 타겟 방향으로 이동, 없으면 발사체 파괴
    /// @TODO: Destroy를 Object Pool에 Release하도록 수정 필요
    /// </summary>
    private void Update()
    {
        if (target != null && target.gameObject.activeSelf)
        {
            Vector3 d = (target.position - transform.position).normalized;
            movement2D.MoveTo(d);
        }
        else
        {
            ReleaseBullet();
        }
    }
}
