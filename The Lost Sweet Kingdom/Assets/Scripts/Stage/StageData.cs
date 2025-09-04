using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "Tower Defense/Stage Data")]
public class StageData : ScriptableObject
{
    public string stageId;       // 고유 id (예: "stage_01")
    public string displayName;   // UI에 표시할 이름
    public string sceneName;     // 실제 씬 로드용 이름
    public Sprite thumbnail;     // 슬롯에 표시할 이미지
    [TextArea] public string description;
    public int difficulty = 1;

    // 기본 잠금 상태 (에디터에서 설정). 실제 플레이어 잠금 상태는 SaveSystem에서 관리.
    public bool defaultLocked = true;
}
