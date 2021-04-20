using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject lvlSelectCanvas;

    public void OpenLevelSelect()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        lvlSelectCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        lvlSelectCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

}
