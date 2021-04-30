using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cashText;
    [SerializeField] TextMeshProUGUI circuitText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] Slider hpBar;
    [SerializeField] Gradient hpGradient;
    [SerializeField] Image hpImg;
    [SerializeField] RawImage itemImage;
    Texture defaultItemImage; 

    // Pause Screen Elements
    bool isPaused = false;
    bool onAltScreen = false; // If a second screen is opened on the pause menu e.g. settings screen.
    [SerializeField] GameObject mainPauseCanvas;    // The main canvas of the pause screen
    [SerializeField] GameObject pauseCanvas;   // The initial screen of the pause screen
    [SerializeField] GameObject confirmCanvas;   // The confirmation screen for quitting the level
    [SerializeField] GameObject pauseBG;   // BG for the pause screen
    [SerializeField] GameObject postProcess;   // Post-processing for the pause screen
    bool isTweenFinished = true;   // Checks if the pause tween is finished

    int maxWave = 10;
    List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (EnemySpawner spawner in FindObjectsOfType<EnemySpawner>())
            enemySpawners.Add(spawner);

        defaultItemImage = itemImage.texture;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveCounter();
    }

    bool isAllWavesCompleted()
    {
        foreach (EnemySpawner spawner in enemySpawners)
            if (!spawner.isFinished)
                return false;

        return true;
    }

    public void UpdateWaveCounter()
    {
        int currentWave = int.MaxValue;

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner._waveIndex < currentWave)
                currentWave = spawner._waveIndex;
        }

        if (isAllWavesCompleted())
            SceneManager.LoadScene(3);

        waveText.text = "Waves Remaining: " + (maxWave - currentWave).ToString();
    }

    public void UpdateCash(Dictionary<string, int> inventory)
    {
        cashText.text = inventory["Bits"].ToString();
        circuitText.text = inventory["Circuits"].ToString();
    }

    public void UpdateItemSlot(GameObject item)
    {
        if (item)
            itemImage.texture = item.GetComponent<Object>().objectImage;
        else
            itemImage.texture = defaultItemImage;
    }

    public void SetMaxHP(int hp)
    {
        hpBar.maxValue = hp;
        hpBar.value = hp;
        hpImg.color = hpGradient.Evaluate(1f);
    }

    public void UpdateHP(int hp)
    {
        hpBar.value = hp;
        hpImg.color = hpGradient.Evaluate(hpBar.normalizedValue);
    }

    // Pauses the game and enables pause screen
    public void PauseGame()
    {
        if (isTweenFinished)
        {
            if (!isPaused)
            {
                Time.timeScale = 0;
                mainPauseCanvas.SetActive(true);
                postProcess.SetActive(true);
                pauseBG.SetActive(true);
                pauseCanvas.SetActive(true);
                isTweenFinished = false;
                LeanTween.move(pauseCanvas.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 0.3f).setEase(LeanTweenType.easeOutExpo).setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    isTweenFinished = true;
                });
                isPaused = !isPaused;
            }
            else
            {
                if (onAltScreen)
                {
                    OpenMainPauseScreen();
                }
                else
                {
                    Time.timeScale = 1;
                    postProcess.SetActive(false);
                    pauseBG.SetActive(false);
                    isTweenFinished = false;
                    LeanTween.move(pauseCanvas.GetComponent<RectTransform>(), new Vector3(0, -Screen.height, 0), 0.25f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
                    {
                        mainPauseCanvas.SetActive(false);
                        pauseCanvas.SetActive(false);
                        isTweenFinished = true;
                    });
                    isPaused = !isPaused;
                }
            }
        }
    }

    // Opens the main pause menu
    // I.e. the one with a resume btn, main menu btn, etc.
    public void OpenMainPauseScreen()
    {
        confirmCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        onAltScreen = false;
    }

    // Opens a confirmation screen to make sure player actually wants to quit
    public void OpenConfirmScreen()
    {
        pauseCanvas.SetActive(false);
        confirmCanvas.SetActive(true);
        onAltScreen = true;
    }
}
