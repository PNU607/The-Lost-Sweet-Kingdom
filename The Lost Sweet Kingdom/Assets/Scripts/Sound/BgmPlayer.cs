using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    public string bgmName;
    private void Start()
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play(bgmName, true);
        _soundObject.SetVolume(0.1f);
    }
}
