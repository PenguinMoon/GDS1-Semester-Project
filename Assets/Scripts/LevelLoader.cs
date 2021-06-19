using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    CanvasGroup fadeImage;
    float fadeSpeed = 0.5f;
    public bool isLoadingLevel = false;
    [SerializeField] TextMeshProUGUI hintTxt;
    [SerializeField] HintText hintData;
    string[] hints;

    private void Awake()
    {
        fadeImage = GetComponentInChildren<CanvasGroup>();
        hints = hintData.Hints;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string levelName)
    {
        hintTxt.text = hints[Random.Range(0, hints.Length)];
        StartCoroutine(FadeLoadLevel(levelName));
    }

    IEnumerator FadeLoadLevel(string levelName)
    {
        isLoadingLevel = true;

        float percent = 0;

        while (percent <= 1)
        {
            percent += Time.unscaledDeltaTime * fadeSpeed;
            fadeImage.alpha = percent;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1.2f);

        SceneManager.LoadScene(levelName);

        yield return new WaitForSecondsRealtime(1f);

        isLoadingLevel = false;

        StartCoroutine(FadeIntoLevel());
    }

    IEnumerator FadeIntoLevel()
    {

        float percent = 1;

        while (percent >= 0)
        {
            percent -= Time.unscaledDeltaTime * fadeSpeed;
            fadeImage.alpha = percent;

            yield return null;
        }
    }
}
