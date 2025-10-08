using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Default Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ShowBounceUI(GameObject uiObject)
    {
        if (uiObject == null)
        {
            Debug.LogWarning("ShowBounceUI: 대상 UI 오브젝트가 없습니다!");
            return;
        }

        uiObject.SetActive(true);

        RectTransform rect = uiObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 1000);

        rect.DOAnchorPos(Vector2.zero, 0.7f)
            .SetEase(Ease.OutBounce)
            .SetUpdate(true);
    }

    public void FadeIn(Image targetImage, float duration = -1f)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("FadeIn: 대상 Image가 없습니다!");
            return;
        }

        float fadeTime = duration > 0 ? duration : fadeDuration;

        Color color = targetImage.color;
        color.a = 0f;
        targetImage.color = color;
        targetImage.gameObject.SetActive(true);

        targetImage.DOFade(1f, fadeTime).SetUpdate(true);
    }

    public void FadeOut(Image targetImage, float duration = -1f, bool deactivateAfter = true)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("FadeOut: 대상 Image가 없습니다!");
            return;
        }

        float fadeTime = duration > 0 ? duration : fadeDuration;

        targetImage.DOFade(0f, fadeTime)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                if (deactivateAfter)
                    targetImage.gameObject.SetActive(false);
            });
    }
    public void FadeInText(Text targetText, float duration = -1f)
    {
        if (targetText == null) return;
        float fadeTime = duration > 0 ? duration : fadeDuration;
        Color c = targetText.color;
        c.a = 0;
        targetText.color = c;
        targetText.gameObject.SetActive(true);
        targetText.DOFade(1f, fadeTime).SetUpdate(true);
    }

    public void FadeOutText(Text targetText, float duration = -1f, bool deactivateAfter = true)
    {
        if (targetText == null) return;
        float fadeTime = duration > 0 ? duration : fadeDuration;
        targetText.DOFade(0f, fadeTime).SetUpdate(true)
            .OnComplete(() =>
            {
                if (deactivateAfter)
                    targetText.gameObject.SetActive(false);
            });
    }

    public void DelayEvent(float delayTime)
    {
        StartCoroutine(StartDelay(delayTime));
    }
    private IEnumerator StartDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }
}
