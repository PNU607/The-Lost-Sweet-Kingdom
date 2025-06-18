using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[TextArea] public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToooltipShow();
        //TooltipManager.Instance.Show(tooltipText, Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToooltipHide();
        //TooltipManager.Instance.Hide();
    }

    protected virtual void ToooltipShow()
    {

    }

    protected virtual void ToooltipHide()
    {

    }
}
