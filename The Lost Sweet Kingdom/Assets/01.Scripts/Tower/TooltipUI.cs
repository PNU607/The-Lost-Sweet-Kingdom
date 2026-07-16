using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipObj;

    public virtual void Awake()
    {
        if (tooltipObj != null)
        {
            tooltipObj.SetActive(false);
        }
    }

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
        tooltipObj.SetActive(true);
    }

    protected virtual void ToooltipHide()
    {
        tooltipObj.SetActive(false);
    }
}
