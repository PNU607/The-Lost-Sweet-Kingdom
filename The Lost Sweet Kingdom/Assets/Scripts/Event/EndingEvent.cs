using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingEvent : MonoBehaviour
{
    [SerializeField] private GameObject titleName;
    [SerializeField] private GameObject towers;
    [SerializeField] private TextMeshPro gameName;
    [SerializeField] private TextMeshPro creater;
    [SerializeField] private TextMeshPro thxText;

    [SerializeField] private float delayTime = 5f;
    [SerializeField] private float shortDelayTime = 2f;

    [SerializeField] private string nextScene;

    private void Start()
    {
        StartCoroutine(EndingCredit());
    }
    private IEnumerator EndingCredit()
    {
        EventManager.Instance.ShowBounceUI(titleName);
        yield return EventManager.Instance.DelayEvent(delayTime);
        EventManager.Instance.FadeOut(titleName.GetComponent<Image>());
        yield return EventManager.Instance.DelayEvent(shortDelayTime);
        EventManager.Instance.FadeIn(towers.GetComponent<Image>());
        yield return EventManager.Instance.DelayEvent(shortDelayTime);
        EventManager.Instance.FadeInTMP(gameName);
        yield return EventManager.Instance.DelayEvent(delayTime);
        EventManager.Instance.FadeOutTMP(gameName);
        yield return EventManager.Instance.DelayEvent(shortDelayTime);
        EventManager.Instance.FadeInTMP(creater);
        yield return EventManager.Instance.DelayEvent(delayTime);
        EventManager.Instance.FadeOutTMP(creater);
        yield return EventManager.Instance.DelayEvent(shortDelayTime);
        EventManager.Instance.FadeOut(towers.GetComponent<Image>());
        yield return EventManager.Instance.DelayEvent(shortDelayTime);
        EventManager.Instance.FadeInTMP(thxText);
        yield return EventManager.Instance.DelayEvent(delayTime);
        EventManager.Instance.FadeOutTMP(thxText);
        yield return EventManager.Instance.DelayEvent(delayTime);

        SceneManager.LoadScene(nextScene);
    }
}
