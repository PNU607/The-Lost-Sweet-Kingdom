/* 
 * @file: Bullet.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: GunTower에서 발사하는 발사체 스크립트
 * @details:
 *  - 적 방향으로 이동하여 적과 충돌하면 대미지를 입힘
 * @see: GunTower.cs
 * @history:
 *  - 2025-02-09: Bullet 스크립트 최초 작성
 */

using UnityEngine;

/* 
 * @class: Bullet
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: GunTower에서 발사하는 발사체 클래스
 * @details:
 *  - 적과 충돌 시 적의 hp를 줄이는 기능
 *  - Movement2D를 이용해 발사체 이동 기능
 * @history:
 *  - 2025-02-09: Bullet 클래스 최초 작성
 */
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 2D 이동
    /// </summary>
    private Movement2D movement2D;

    /// <summary>
    /// 발사체를 맞출 목표물
    /// </summary>
    private Transform target;

    /// <summary>
    /// 적에게 줄 대미지
    /// </summary>
    private float attackDamage;

    /// <summary>
    /// 발사체가 날아가는 방향 (적의 방향)
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// 발사체 세팅
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackDamage"></param>
    public void Setup(Transform target, float attackDamage)
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;
        this.attackDamage = attackDamage;

        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
    }

    /// <summary>
    /// 업데이트
    /// 타겟이 있으면 발사체가 타겟 방향으로 이동, 없으면 발사체 파괴
    /// </summary>
    private void Update()
    {
        if (target != null && target.gameObject.activeSelf)
        {
            movement2D.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    /// <summary>
    /// 충돌 체크
    /// 충돌한 collision의 태그가 Enemy이면, 타겟에 대미지를 주고 충돌체는 파괴
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        //if (collision.transform != target) return;

        collision.GetComponent<EnemyTest>().TakeDamage(attackDamage);
        Destroy(gameObject);
    }
}
