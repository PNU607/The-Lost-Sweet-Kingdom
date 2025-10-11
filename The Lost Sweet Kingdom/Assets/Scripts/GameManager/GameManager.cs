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

    public bool isCleared = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
