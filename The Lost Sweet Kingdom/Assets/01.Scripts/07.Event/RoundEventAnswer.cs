using TMPro;
using UnityEngine;

public class RoundEventAnswer : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField] private TMP_Text label;
    public CanvasGroup Group => group;
    public UnityEngine.UI.Button Button => button;

    public void Bind(string text, bool enabled)
    {
        label.text = text;
        button.interactable = enabled;
        group.blocksRaycasts = enabled;
        button.targetGraphic.raycastTarget = enabled;
    }
}
