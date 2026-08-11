using UnityEngine;

[CreateAssetMenu(fileName = "NewStageData", menuName = "Tower Defense/Stage Data")]
public class StageData : ScriptableObject
{
    public string stageName;
    public Sprite stageSprite;
    public DifficultyLevel difficulty;
    public bool isLocked;
    public bool isCleared;
}

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard,
    Expert
}
