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

    public void OpenLevelSelect()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        lvlSelectCanvas.SetActive(true);
        mainCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(btn[1]);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        lvlSelectCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(btn[0]);
    }

}
