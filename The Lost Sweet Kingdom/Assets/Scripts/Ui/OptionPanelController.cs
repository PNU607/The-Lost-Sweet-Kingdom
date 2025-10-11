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
            Time.timeScale = 0f;
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

            Time.timeScale = 1f;

            optionPanel.SetActive(false);
        }
    }
}
