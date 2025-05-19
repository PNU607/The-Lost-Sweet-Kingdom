using System.Collections;
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
        { SoundType.EFFECT, 1f },
        { SoundType.FRIENDLY, 1f },
        { SoundType.ENEMY, 1f },
        { SoundType.BGM, 1f }
    };
}
