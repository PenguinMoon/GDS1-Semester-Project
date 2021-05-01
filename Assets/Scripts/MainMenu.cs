using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    enum Screen { Main, LvlSelect, Options };

    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject lvlSelectCanvas;
    [SerializeField] GameObject optionsCanvas;

    Screen currentScreen = Screen.Main;

    [Header("First Highlighted Button on Each Screen")]
    [SerializeField] GameObject[] btn;

    [SerializeField] ScrollRectScrolling scrollRect;

    [SerializeField] GameObject mainCam;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            EventSystem.current.SetSelectedGameObject(btn[0]);
        });
    }

    public void OpenLevelSelect()
    {
        currentScreen = Screen.LvlSelect;
        LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            mainCanvas.SetActive(false);
        });

        LeanTween.rotate(mainCam, new Vector3(5, 90, 0), .9f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            lvlSelectCanvas.SetActive(true);
            LeanTween.scale(lvlSelectCanvas.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(btn[1]);
                scrollRect.currBtn = btn[1];
                scrollRect.nextBtn = btn[1];
            });
            scrollRect.ResetRect();
        });
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        if (currentScreen == Screen.LvlSelect)
        {
            LeanTween.scale(lvlSelectCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                lvlSelectCanvas.SetActive(false);
            });
        }
        else
        {
            LeanTween.scale(optionsCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                optionsCanvas.SetActive(false);
            });
        }

        LeanTween.rotate(mainCam, new Vector3(5, 0, 0), .9f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            mainCanvas.SetActive(true);
            LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(btn[0]);
            });
        });
    }

    public void OpenOptionsMenu()
    {
        currentScreen = Screen.Options;
        LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            mainCanvas.SetActive(false);
        });

        LeanTween.rotate(mainCam, new Vector3(5, -90, 0), .9f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            optionsCanvas.SetActive(true);
            LeanTween.scale(optionsCanvas.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(btn[2]);
            });
        });
    }
}
