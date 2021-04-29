using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject lvlSelectCanvas;

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
        //mainCanvas.SetActive(false);
        LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad);

        LeanTween.rotate(mainCam, new Vector3(0, 90, 0), .9f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            //lvlSelectCanvas.SetActive(true);
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
        //lvlSelectCanvas.SetActive(false);
        LeanTween.scale(lvlSelectCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), .5f).setEase(LeanTweenType.easeOutQuad);

        LeanTween.rotate(mainCam, new Vector3(0, 0, 0), .9f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            //mainCanvas.SetActive(true);
            LeanTween.scale(mainCanvas.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(btn[0]);
            });
        });
    }

}
