using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "Tower Defense/Stage Data")]
public class StageData : ScriptableObject
{
    public string stageId;       // ���� id (��: "stage_01")
    public string displayName;   // UI�� ǥ���� �̸�
    public string sceneName;     // ���� �� �ε�� �̸�
    public Sprite thumbnail;     // ���Կ� ǥ���� �̹���
    [TextArea] public string description;
    public int difficulty = 1;

    // �⺻ ��� ���� (�����Ϳ��� ����). ���� �÷��̾� ��� ���´� SaveSystem���� ����.
    public bool defaultLocked = true;
}
