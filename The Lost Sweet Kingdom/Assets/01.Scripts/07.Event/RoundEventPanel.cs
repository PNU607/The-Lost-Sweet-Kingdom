using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RoundEventPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private CanvasGroup contentGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private UnityEngine.UI.Image art;
    [SerializeField] private Transform answersRoot;
    [SerializeField] private RoundEventAnswer answerTemplate;
    [SerializeField] private UnityEngine.UI.Button continueButton;
    [SerializeField] private float riseDistance = 100;
    [SerializeField] private float revealDuration = .28f;
    [SerializeField] private float answerDelay = .1f;

    public bool Ready { get; private set; }
    private RoundEventController owner;
    private readonly List<RoundEventAnswer> answers = new();
    private IReadOnlyList<ResolvedEventChoice> choices;
    private Sequence sequence;
    private Sprite defaultArt;
    private Vector2 panelPosition;
    private Vector2 contentPosition;

    private void Awake()
    {
        defaultArt = art.sprite;
        panelPosition = ((RectTransform)panelGroup.transform).anchoredPosition;
        contentPosition = ((RectTransform)contentGroup.transform).anchoredPosition;
        continueButton.onClick.AddListener(() => owner.Continue());
        answerTemplate.gameObject.SetActive(false);
    }

    public void Show(RoundEventController controller, RoundEventData data, IReadOnlyList<ResolvedEventChoice> resolved, int round)
    {
        owner = controller;
        choices = resolved;
        gameObject.SetActive(true);
        sequence?.Kill();
        Ready = false;
        titleText.text = data.title;
        descriptionText.text = data.description;
        statusText.text = $"{round}라운드의 만남";
        art.sprite = string.IsNullOrWhiteSpace(data.artResource) ? defaultArt : Resources.Load<Sprite>(data.artResource);
        if (art.sprite == null) art.sprite = defaultArt;
        continueButton.gameObject.SetActive(false);
        foreach (var answer in answers) { answer.gameObject.SetActive(false); Destroy(answer.gameObject); }
        answers.Clear();
        for (int i = 0; i < choices.Count; i++)
        {
            int index = i;
            var answer = Instantiate(answerTemplate, answersRoot);
            answer.gameObject.SetActive(true);
            answer.Button.onClick.AddListener(() => owner.Choose(index));
            answers.Add(answer);
        }
        RefreshChoices(null);
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)answersRoot);
        panelGroup.interactable = false;
        sequence = DOTween.Sequence().SetUpdate(true);
        Reveal(sequence, panelGroup, panelPosition);
        Reveal(sequence, contentGroup, contentPosition);
        foreach (var answer in answers)
        {
            // Animate inside a layout-owned row so the LayoutGroup never fights the tween.
            sequence.AppendInterval(answerDelay);
            Reveal(sequence, answer.Group, Vector2.zero);
        }
        sequence.OnComplete(() => { Ready = true; panelGroup.interactable = true; RefreshChoices(null); });
    }

    private void Reveal(Sequence tween, CanvasGroup group, Vector2 destination)
    {
        var rect = (RectTransform)group.transform;
        rect.anchoredPosition = destination + Vector2.up * riseDistance;
        group.alpha = 0;
        tween.Append(rect.DOAnchorPos(destination, revealDuration).SetEase(Ease.OutCubic));
        tween.Join(group.DOFade(1, revealDuration));
    }

    public void RefreshChoices(string status)
    {
        if (status != null) statusText.text = status;
        for (int i = 0; i < answers.Count; i++)
        {
            string reason = owner.DisabledReason(choices[i]);
            answers[i].Bind(reason ?? choices[i].Text, reason == null);
        }
    }

    public void ShowResult(string result)
    {
        statusText.text = "선택 결과";
        descriptionText.text = result;
        foreach (var answer in answers) answer.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(true);
        continueButton.Select();
    }

    public void Hide()
    {
        sequence?.Kill();
        Ready = false;
        gameObject.SetActive(false);
    }

    private void OnDestroy() => sequence?.Kill();
}
