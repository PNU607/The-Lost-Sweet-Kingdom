using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float sceneChangeDuration = 5;
    [SerializeField] private string nextScene;

    public void Fadeout()
    {
        if (blackScreen != null)
        {
            blackScreen.SetActive(true);
        }
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float time = 0f;
        Image fadeImage = blackScreen.GetComponent<Image>();
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(sceneChangeDuration);
        SceneManager.LoadScene(nextScene);
    }
}
