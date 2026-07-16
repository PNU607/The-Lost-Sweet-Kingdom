using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusIconUI : TooltipUI
{
    [SerializeField]
    private Image bonusImage;
    [SerializeField]
    private TextMeshProUGUI bonusNameText;
    [SerializeField]
    private TextMeshProUGUI bonusContentText;

    public void SetBonus(BonusData bonusData)
    {
        bonusImage.sprite = bonusData.bonusIcon;
        bonusNameText.text = bonusData.bonusName;
        bonusContentText.text = bonusData.bonusContent;
    }

    protected override void ToooltipShow()
    {
        base.ToooltipShow();
    }

    override protected void ToooltipHide()
    {
        base.ToooltipHide();
    }
}
