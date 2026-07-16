using UnityEngine;
using System.Sound;
public class BgmPlayer : MonoBehaviour
{
    public string bgmName;

    private void Start()
    {
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(bgmName))
        {
            SoundManager.Instance.PlayBgm(bgmName);
        }
    }
}
