using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneLoader : MonoBehaviour
{
    public void OnButtonPressed(string lvlName)
    {
        FindObjectOfType<LevelLoader>().LoadLevel(lvlName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
