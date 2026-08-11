using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionPanelController : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject dimPanel;

    public void OpenPanel()
    {
        if (optionPanel != null)
        {
            SoundObject _soundObject;
            _soundObject = Sound.Play("TowerUIMoushover", false);

            if (GameManager.Instance != null) GameManager.Instance.StopGame();

            //_soundObject.SetVolume(8f);

            if (dimPanel != null)
            {
                dimPanel.SetActive(true);
                Image dim = dimPanel.GetComponent<Image>();
                dim.DOFade(0.5f, 0.5f).SetUpdate(true);
            }

            optionPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (optionPanel != null)
        {
            SoundObject _soundObject;
            _soundObject = Sound.Play("TowerUIMoushover", false);
            //_soundObject.SetVolume(8f);

            if (dimPanel != null)
            {
                Debug.Log("SetActiveFalse dim");
                Image dim = dimPanel.GetComponent<Image>();
                Color dimColor = dim.color;
                dimColor.a = 0f;
                dim.color = dimColor;

                dimPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Dimpanel Null");
            }

            if (GameManager.Instance != null) GameManager.Instance.ResumeGame();

            optionPanel.SetActive(false);
        }
    }
}
