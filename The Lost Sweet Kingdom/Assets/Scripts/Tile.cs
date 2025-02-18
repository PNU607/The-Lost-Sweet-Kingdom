/* 
 * @file: Tile.cs
 * @author: 서지혜
 * @date: 2025-02-10
 * @brief: 타워를 배치할 수 있는 타일 스크립트
 * @details:
 *  - 타일에 타워가 지어져 있는지 확인
 * @see: TowerManager.cs
 * @history:
 *  - 2025-02-10: Tile 스크립트 최초 작성
 */


using UnityEngine;

/* 
 * @class: Tile
 * @author: 서지혜
 * @date: 2025-02-10
 * @brief: 타워를 배치할 수 있는 타일 클래스
 * @details:
 *  - 타일 점유 유무를 변수로 확인 가능
 * @history:
 *  - 2025-02-10: Tile 클래스 최초 작성
 */
public class Tile : MonoBehaviour
{
    /// <summary>
    /// 타일의 점유 여부
    /// </summary>
    public bool isOccupied { set; get; }

    /// <summary>
    /// Awake
    /// 점유 여부 false로 변경
    /// </summary>
    private void Awake()
    {
        isOccupied = false;
    }
}
