using UnityEngine;

public class StageSelectorInputHandler : MonoBehaviour
{

    void Update()
    {
        // Ű���� �Է� ó��
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            // �������� ȸ��
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            // ���������� ȸ��
        }

        // ���� Ȯ��
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // ���� ���� ���� �߰�
        }
    }
}