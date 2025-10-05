using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Sound;

public enum SoundType
{
    UI,
    BGM
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private const float DEFAULT_VOLUME = 1;

    private SoundObject bgmSoundObject;

    public SoundSourceList soundSourceList;
    private Dictionary<SoundType, float> volumes = new Dictionary<SoundType, float>();
    public List<SoundObject> soundObjects;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeVolumes();

        var soundData = DataManager.Instance.SoundData;
        SetMasterVolume(soundData.masterVolume);
        foreach (var entry in soundData.typeVolumes)
        {
            SetVolume(entry.type, entry.volume);
        }
    }

    private void InitializeVolumes()
    {
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            volumes[type] = DEFAULT_VOLUME;
        }
    }

    public SoundObject GetBgmObject()
    {
        if (bgmSoundObject == null)
        {
            bgmSoundObject = gameObject.AddComponent<SoundObject>();
            bgmSoundObject.enabled = true;
        }

        return bgmSoundObject;
    }

    public void PlayBgm(string bgmName)
    {
        if (bgmSoundObject != null && bgmSoundObject.IsPlaying)
        {
            if (bgmSoundObject.name == bgmName)
                return;

            bgmSoundObject.Stop();
        }

        bgmSoundObject = Sound.Play(bgmName, true);
        bgmSoundObject.name = bgmName;
    }

    private void Start()
    {
        GetSoundObjects();
    }

    private void GetSoundObjects()
    {
        soundObjects = new List<SoundObject>(FindObjectsOfType<SoundObject>());
    }

    public void SetVolume(SoundType type, float volume)
    {
        volumes[type] = volume;

        foreach (SoundObject soundObject in soundObjects)
        {
            if (soundObject.GetSoundType() == type)
            {
                soundObject.SetVolume(volume);
            }
        }
    }

    public void SetMasterVolume(float masterVolume)
    {
        foreach (SoundObject soundObject in soundObjects)
        {
            soundObject.SetMasterVolume(masterVolume);
        }
    }

    public float GetVolume(SoundType type)
    {
        try
        {
            return volumes[type];
        }
        catch (KeyNotFoundException)
        {
            InitializeVolumes();
            return volumes[type];
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetSoundObjects();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
