using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    CanvasGroup fadeImage;
    float fadeSpeed = 0.5f;
    public bool isLoadingLevel = false;

    private void Awake()
    {
        fadeImage = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(FadeLoadLevel(levelName));
    }

    IEnumerator FadeLoadLevel(string levelName)
    {
        isLoadingLevel = true;

        float percent = 0;

        while (percent <= 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            fadeImage.alpha = percent;

            yield return null;
        }

        SceneManager.LoadScene(levelName);

        yield return new WaitForSeconds(1f);

        isLoadingLevel = false;

        StartCoroutine(FadeIntoLevel());
    }

    IEnumerator FadeIntoLevel()
    {

        float percent = 1;

        while (percent >= 0)
        {
            percent -= Time.deltaTime * fadeSpeed;
            fadeImage.alpha = percent;

            yield return null;
        }
    }
}
