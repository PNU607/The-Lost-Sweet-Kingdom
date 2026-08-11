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
        uiVolumeSlider.value = soundData.GetVolume(SoundType.UI);
        bgmVolumeSlider.value = soundData.GetVolume(SoundType.BGM);

        SoundManager.Instance.SetMasterVolume(soundData.masterVolume);
        foreach (var entry in soundData.typeVolumes)
        {
            SoundManager.Instance.SetVolume(entry.type, entry.volume);
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
