using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Sound;

public class GameStart : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    public void OnButtonClick()
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play("TowerUIMoushover", false);
        //_soundObject.SetVolume(0.1f);
        Time.timeScale = 1f;

        SceneManager.LoadScene(nextSceneName);
    }
}
