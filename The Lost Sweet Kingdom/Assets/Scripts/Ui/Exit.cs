using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System.Sound;

public class Exit : MonoBehaviour
{
    public void OnButtonClick()
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play("TowerUIMoushover", false);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
