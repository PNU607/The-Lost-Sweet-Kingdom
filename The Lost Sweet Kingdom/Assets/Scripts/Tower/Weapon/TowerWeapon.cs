/* 
 * @file: TowerWeapon.cs
 * @author: 서지혜
 * @date: 2025-03-23
 * @brief: 타워의 공격 무기 클래스
 * @details:
 *  - 타워의 공격을 위해 필요한 기능
 * @see: Tower.cs
 * @history:
 *  - 2025-03-23: TowerWeapon 스크립트 최초 작성
 */

using UnityEngine;

/* 
 * @class: TowerWeapon
 * @author: 서지혜
 * @date: 2025-03-23
 * @brief: 타워의 공격 무기 클래스
 * @details:
 *  - 타워가 적에게 공격하기 위해 필요한 기능들
 * @history:
 *  - 2025-03-23: TowerWeapon 클래스 최초 작성
 */
public class TowerWeapon : MonoBehaviour
{
    /// <summary>
    /// 공격 목표물
    /// </summary>
    protected Transform target;

    /// <summary>
    /// Weapon을 발사하는 타워
    /// </summary>
    protected Tower shotTower;

    /// <summary>
    /// Tower의 공격이 멈췄는지 여부
    /// </summary>
    protected bool isAttackStopped = false;

    protected bool isSetup = false;

    /// <summary>
    /// 발사체 세팅
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackDamage"></param>
    public virtual void Setup(Transform target, Tower shotTower)
    {
        this.target = target;
        this.shotTower = shotTower;
        isSetup = true; // 세팅 완료 상태로 변경
    }

    /// <summary>
    /// 업데이트
    /// </summary>
    protected virtual void Update()
    {
        if (!isSetup)
        {
            return;
        }
    }

    /// <summary>
    /// Bullet을 Object Pool로 되돌림
    /// </summary>
    protected virtual void ReleaseWeapon()
    {
        isSetup = false;

        if (shotTower != null)
        {
            shotTower.GetComponent<Tower>().ReleaseWeapon(this);
            target = null; // 타겟을 초기화하여 더 이상 공격하지 않도록 함
            shotTower = null; // 발사한 타워를 초기화
        }
        else
        {
            Debug.Log("shotTower 없음");
        }
    }

    /// <summary>
    /// 발사체의 공격 기능
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void Attack(Collider2D collision)
    {
        
    }
}
