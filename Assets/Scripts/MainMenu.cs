using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    enum MenuScreen { Main, LvlSelect, Options, Credits };

    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject lvlSelectCanvas;
    [SerializeField] GameObject optionsCanvas;
    [SerializeField] GameObject creditsCanvas;

    MenuScreen currentScreen = MenuScreen.Main;

    [Header("First Highlighted Button on Each Screen")]
    [SerializeField] GameObject[] btn;

    [SerializeField] ScrollRectScrolling scrollRect;

    [SerializeField] GameObject mainCam;

    [Header("Audio Stuff")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip btnSound;

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
        audioSource.PlayOneShot(btnSound);
        currentScreen = MenuScreen.LvlSelect;
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
        audioSource.PlayOneShot(btnSound);
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        audioSource.PlayOneShot(btnSound);
        if (currentScreen == MenuScreen.LvlSelect)
        {
            LeanTween.scale(lvlSelectCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                lvlSelectCanvas.SetActive(false);
            });
        }
        else if (currentScreen == MenuScreen.Options)
        {
            LeanTween.scale(optionsCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                optionsCanvas.SetActive(false);
            });
        }
        else
        {
            LeanTween.move(creditsCanvas.GetComponent<RectTransform>(), new Vector3(0, -Screen.height, 0), 0.5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                creditsCanvas.SetActive(false);
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
        audioSource.PlayOneShot(btnSound);
        currentScreen = MenuScreen.Options;
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

    public void OpenCredits()
    {
        audioSource.PlayOneShot(btnSound);
        currentScreen = MenuScreen.Credits;
        LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            mainCanvas.SetActive(false);
        });
        LeanTween.rotate(mainCam, new Vector3(90, 0, 0), .9f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            creditsCanvas.SetActive(true);
            LeanTween.move(creditsCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 0.4f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(btn[3]);
            });
        });
    }
}
