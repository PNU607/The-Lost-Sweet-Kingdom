using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private const string FileName = "gameData.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    public SoundData SoundData { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            SoundData = JsonUtility.FromJson<SoundData>(json);
        }
        else
        {
            SoundData = new SoundData();
            SaveData();
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(SoundData, true);
        File.WriteAllText(SavePath, json);
    }

    public void SetMasterVolume(float volume)
    {
        SoundData.masterVolume = volume;
        SaveData();
    }

    public void SetVolume(SoundType type, float volume)
    {
        if (SoundData.typeVolumes.ContainsKey(type))
        {
            SoundData.typeVolumes[type] = volume;
        }
        else
        {
            SoundData.typeVolumes.Add(type, volume);
        }
        SaveData();
    }

    public float GetMasterVolume()
    {
        return SoundData.masterVolume;
    }

    public float GetVolume(SoundType type)
    {
        return SoundData.typeVolumes.TryGetValue(type, out var value) ? value : 1f;
    }
}
