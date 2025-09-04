using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public float masterVolume = 1f;

    [SerializeField]
    public Dictionary<SoundType, float> typeVolumes = new Dictionary<SoundType, float>()
    {
        { SoundType.UI, 1f },
        { SoundType.BGM, 1f }
    };

    public float GetVolume(SoundType type)
    {
        return typeVolumes.ContainsKey(type) ? typeVolumes[type] : 1f;
    }

    public void SetVolume(SoundType type, float value)
    {
        if (typeVolumes.ContainsKey(type))
            typeVolumes[type] = value;
    }
}
