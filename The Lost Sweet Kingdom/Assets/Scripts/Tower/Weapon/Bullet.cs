/* 
 * @file: Bullet.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: GunTower에서 발사하는 일직선 탄 발사체 스크립트
 * @details:
 *  - 처음 노렸을 때의 적 방향으로 직선 이동하여 적과 충돌하면 데미지를 입힘
 * @see: GunTower.cs
 * @history:
 *  - 2025-02-09: Bullet 스크립트 최초 작성
 *  - 2025-03-23: TowerWeapon 추가로 최상위 기능 분리 및 무기의 이동 기능 로직 수정
 */

using UnityEngine;

/* 
 * @class: Bullet
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: GunTower에서 발사하는 일직선 탄 발사체 클래스
 * @details:
 *  - 적과 충돌 시 적의 hp를 줄이는 기능
 *  - Movement2D를 이용해 발사체 일직선 이동 기능
 * @history:
 *  - 2025-02-09: Bullet 클래스 최초 작성
 *  - 2025-03-23: TowerWeapon 상속으로 인해 Setup 함수 수정, Update 함수 삭제 및 MoveWeapon 추가
 */
public class Bullet : TowerWeapon
{
    /// <summary>
    /// 2D 이동
    /// </summary>
    protected Movement2D movement2D;

    /// <summary>
    /// 발사체가 날아가는 방향 (적의 방향)
    /// </summary>
    protected Vector3 direction;

    /// <summary>
    /// 발사체 세팅
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackDamage"></param>
    public override void Setup(Transform target, Tower shotTower)
    {
        base.Setup(target, shotTower);

        movement2D = GetComponent<Movement2D>();

        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
    }

    protected override void MoveWeapon()
    {
        base.MoveWeapon();

        movement2D.MoveTo(direction);
    }

    /// <summary>
    /// 충돌 체크
    /// 충돌한 collision의 태그가 Enemy이면, 타겟에 데미지를 주고 충돌체는 파괴
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        //if (collision.transform != target) return;

        if (!this.gameObject.activeSelf) return;
        Attack(collision);
        ReleaseWeapon();
    }

    /// <summary>
    /// 발사체의 공격 기능
    /// </summary>
    /// <param name="collision"></param>
    protected override void Attack(Collider2D collision)
    {
        base.Attack(collision);

        collision.GetComponent<EnemyTest>().TakeDamage(shotTower.CurrentTowerData.levelDatas[shotTower.towerLevel].attackDamage);
    }
}
