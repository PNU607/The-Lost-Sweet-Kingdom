using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Sound;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject dimPanel;
    public bool isSpeedUp = false;
    public float gameSpeed = 1f;

    public bool isCleared = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void IsSpeedUp()
    {
        isSpeedUp = !isSpeedUp;
        if (isSpeedUp)
        {
            gameSpeed = 2f;
            Time.timeScale = gameSpeed;
        }
        else
        {
            gameSpeed = 1f;
            Time.timeScale = gameSpeed;
        }
    }
    public void ResumeGame()
    {
        Time.timeScale = gameSpeed;
    }
    public void StopGame()
    {
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        SoundObject _soundObject;
        _soundObject = Sound.Play("GameOver", false);

        if (dimPanel != null)
        {
            dimPanel.SetActive(true);
            Image dim = dimPanel.GetComponent<Image>();
            dim.DOFade(0.5f, 0.5f).SetUpdate(true);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            RectTransform ui = gameOverUI.GetComponent<RectTransform>();
            ui.anchoredPosition = new Vector2(0, 1000);
            ui.DOAnchorPos(Vector2.zero, 0.7f).SetEase(Ease.OutBounce).SetUpdate(true);
        }
    }
}
