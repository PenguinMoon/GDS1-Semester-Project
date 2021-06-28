using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] string lvlName;
    [SerializeField] TextMeshProUGUI scoreTxt;  // High score text

    private void Awake()
    {
        scoreTxt.text = "High Score : " + PlayerPrefs.GetFloat(lvlName, 0f).ToString("F0");
    }

    public void LoadLevel()
    {
        if (lvlName != "")
        {
            FindObjectOfType<LevelLoader>().LoadLevel(lvlName);
        }
        else
        {
            Debug.LogWarning("Level Name not added to the button!");
        }
    }
}
