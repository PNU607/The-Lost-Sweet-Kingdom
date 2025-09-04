using System.Collections;
using System.Collections.Generic;
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
            optionPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
    }
}
