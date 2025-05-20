using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;

public class BgmPlayer : MonoBehaviour
{
    private void Start()
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play("GameBgm", true);
        _soundObject.SetVolume(0.1f);
    }
}
