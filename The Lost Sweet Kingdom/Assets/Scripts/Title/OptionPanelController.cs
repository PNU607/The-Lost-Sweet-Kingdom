using System.Collections;
using System.Collections.Generic;
using System.Sound;
using UnityEngine;

public class OptionPanelController : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;

    private void Start()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (optionPanel != null)
        {
            SoundObject _soundObject;
            _soundObject = Sound.Play("TowerUIMoushover", false);

            optionPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (optionPanel != null)
        {
            SoundObject _soundObject;
            _soundObject = Sound.Play("TowerUIMoushover", false);

            optionPanel.SetActive(false);
        }
    }
}
