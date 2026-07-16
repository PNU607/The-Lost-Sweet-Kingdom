/* 
 * @file: Movement2D.cs
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 2D 오브젝트의 이동을 담당하는 스크립트
 * @details:
 *  - 지정한 속도로 이동 방향을 받아와 오브젝트를 이동시킴
 * @history:
 *  - 2025-02-09: Movement2D 스크립트 최초 작성
 */

using UnityEngine;

/* 
 * @class: Movement2D
 * @author: 서지혜
 * @date: 2025-02-09
 * @brief: 2D 오브젝트의 이동을 담당하는 클래스
 * @details:
 *  - 이동 방향을 받아 지정한 속도로 이동 기능
 * @history:
 *  - 2025-02-09: Movement2D 클래스 최초 작성
 */
public class Movement2D : MonoBehaviour
{
    /// <summary>
    /// 이동 속도
    /// </summary>
    [SerializeField]
    private float moveSpeed = 0.5f;
    public float MoveSpeed => moveSpeed;

    /// <summary>
    /// 이동 방향
    /// </summary>
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    /// <summary>
    /// 업데이트
    /// 지정한 방향으로 지정한 속도로 이동
    /// </summary>
    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        RotateTo();
    }

    /// <summary>
    /// 이동할 방향을 넘겨받아 오브젝트가 이동하도록 세팅
    /// </summary>
    /// <param name="direction">이동할 방향</param>
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }

    /// <summary>
    /// 오브젝트의 아래쪽이 타겟을 향하도록 회전
    /// </summary>
    /// <param name="direction">타겟의 방향</param>
    public void RotateTo()
    {
        // 회전: 아래쪽이 타겟을 향하도록 (즉, -transform.up이 target 방향)
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 이동
        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
    }
}
