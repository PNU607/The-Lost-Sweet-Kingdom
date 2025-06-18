using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonusIconUI : TooltipUI
{
    [SerializeField]
    private TextMeshProUGUI bonusNameText;
    private TextMeshProUGUI bonusContentText;

    public void SetBonus(string bonusName, string bonusContent)
    {
        bonusNameText.text = bonusName;
        bonusContentText.text = bonusContent;
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
