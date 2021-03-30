using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneLoader : MonoBehaviour
{
    public void OnButtonPressed(int sceneIndexToLoad)
    {
        SceneManager.LoadScene(sceneIndexToLoad);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
