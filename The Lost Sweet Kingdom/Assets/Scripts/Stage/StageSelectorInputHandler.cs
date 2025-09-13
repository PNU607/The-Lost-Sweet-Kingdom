using UnityEngine;

public class StageSelectorInputHandler : MonoBehaviour
{

    void Update()
    {
        // 키보드 입력 처리
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            // 왼쪽으로 회전
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            // 오른쪽으로 회전
        }

        // 선택 확인
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // 게임 시작 로직 추가
        }
    }
}