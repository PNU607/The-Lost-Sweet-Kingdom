using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    public void OnButtonClick()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
