using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Retry : MonoBehaviour
{
    public void OnButtonClick()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        Time.timeScale = 1f;

        SceneManager.LoadScene(currentScene);
    }
}
