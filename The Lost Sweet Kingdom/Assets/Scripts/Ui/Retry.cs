using System.Collections;
using System.Collections.Generic;
using System.Sound;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
    public void OnButtonClick()
    {
        SoundObject _soundObject;
        _soundObject = Sound.Play("TowerUIMoushover", false);

        string currentScene = SceneManager.GetActiveScene().name;

        Time.timeScale = 1f;

        SceneManager.LoadScene(currentScene);
    }
}
