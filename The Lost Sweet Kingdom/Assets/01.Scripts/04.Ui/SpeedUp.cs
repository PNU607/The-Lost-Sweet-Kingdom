using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedUp : MonoBehaviour
{
    [SerializeField]public TextMeshProUGUI speedText;
    private void Start()
    {
        UpdateSpeedUpButtonUI();
    }
    public void OnButtonClick()
    {
        BattleManager.Instance.IsSpeedUp();

        UpdateSpeedUpButtonUI();
    }
    public void UpdateSpeedUpButtonUI()
    {
        if (speedText == null)
        {
            Debug.LogError("Text 없음");
            return;
        }
        if (BattleManager.Instance.isSpeedUp) speedText.text = "x2";
        else speedText.text = "x1";
    }
}
