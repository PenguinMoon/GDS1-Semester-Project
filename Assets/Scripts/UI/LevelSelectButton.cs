using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] string lvlName;
    [SerializeField] TextMeshProUGUI scoreTxt;  // High score text
    [SerializeField] Image star;  // Star that appears if the level has been passed with no damage taken

    private void Awake()
    {
        scoreTxt.text = "High Score : " + PlayerPrefs.GetFloat(lvlName, 0f).ToString("F0");

        // Shows the star if the level has been cleared with no damage taken
        // Else, have the colour of the star be in black
        if (PlayerPrefs.GetInt(lvlName + "Star", 0) == 1)
            star.color = new Color(1f, 1f, 1f);
        else
            star.color = new Color(0f, 0f, 0f);
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
