using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundVolumeEntry
{
    public SoundType type;
    public float volume;
}

[Serializable]
public class SoundData
{
    public float masterVolume = 1f;

    [SerializeField]
    public List<SoundVolumeEntry> typeVolumes = new List<SoundVolumeEntry>()
    {
        new SoundVolumeEntry { type = SoundType.UI, volume = 1f },
        new SoundVolumeEntry { type = SoundType.BGM, volume = 1f }
    };

    public float GetVolume(SoundType type)
    {
        var entry = typeVolumes.Find(e => e.type == type);
        return entry != null ? entry.volume : 1f;
    }

    public void SetVolume(SoundType type, float value)
    {
        var entry = typeVolumes.Find(e => e.type == type);
        if (entry != null)
            entry.volume = value;
        else
            typeVolumes.Add(new SoundVolumeEntry { type = type, volume = value });
    }
}