using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "Tower Defense/Stage Data")]
public class StageData : ScriptableObject
{
    public string stageName;
    public int difficulty;
    public bool isLocked;
}
