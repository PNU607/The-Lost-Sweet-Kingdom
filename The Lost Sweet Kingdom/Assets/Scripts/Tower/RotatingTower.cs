/* 
 * @file: RotatingTower.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 지속적으로 회전하는 타워 스크립트
 * @details:
 *  - 지속적인 회전 상태로 세팅하는 기능
 * @see: Tower.cs
 * @history:
 *  - 2025-02-09: RotatingTower 스크립트 최초 작성
 */

/* 
 * @class: RotatingTower
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 지속적으로 회전하는 타워 클래스
 * @details:
 *  - 지속적인 회전 기능
 * @history:
 *  - 2025-02-09: RotatingTower 클래스 최초 작성
 */
public class RotatingTower : Tower
{
    /// <summary>
    /// 타워의 상태를 지속적으로 회전하는 상태로 세팅
    /// </summary>
    /// <param name="enemyManager"></param>
    public override void Setup(TowerData towerData = null)
    {
        base.Setup(towerData);
        ChangeState(TowerState.Rotate);
    }
}
