using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    [Header("UI Scrollbars")]
    public Scrollbar masterVolumeSlider;
    public Scrollbar uiVolumeSlider;
    public Scrollbar bgmVolumeSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitialSetting();
    }

    public void InitialSetting()
    {
        SoundData soundData = DataManager.Instance.SoundData;

        masterVolumeSlider.value = soundData.masterVolume;
        uiVolumeSlider.value = soundData.typeVolumes[SoundType.UI];
        bgmVolumeSlider.value = soundData.typeVolumes[SoundType.BGM];

        SoundManager.Instance.SetMasterVolume(soundData.masterVolume);
        foreach (var kv in soundData.typeVolumes)
        {
            SoundManager.Instance.SetVolume(kv.Key, kv.Value);
        }

        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        uiVolumeSlider.onValueChanged.AddListener(UpdateUIVolume);
        bgmVolumeSlider.onValueChanged.AddListener(UpdateBgmVolume);
    }

    private void UpdateMasterVolume(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
        DataManager.Instance.SetMasterVolume(value);
    }

    private void UpdateUIVolume(float value)
    {
        SoundManager.Instance.SetVolume(SoundType.UI, value);
        DataManager.Instance.SetVolume(SoundType.UI, value);
    }

    private void UpdateBgmVolume(float value)
    {
        SoundManager.Instance.SetVolume(SoundType.BGM, value);
        DataManager.Instance.SetVolume(SoundType.BGM, value);
    }
}
