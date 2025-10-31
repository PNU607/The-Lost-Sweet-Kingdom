using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    public void OnButtonClick()
    {
        GameManager.Instance.IsSpeedUp();
    }
}
