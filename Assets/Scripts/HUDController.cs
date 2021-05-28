using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cashText;
    [SerializeField] TextMeshProUGUI circuitText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] Slider hpBar;
    [SerializeField] Gradient hpGradient;
    [SerializeField] Image hpImg;

    [SerializeField] TextMeshProUGUI pressToJoinTxt;

    // Pause Screen Elements
    bool isPaused = false;
    bool isFastForward = false;
    float currentTimeScale = 1.0f;
    [SerializeField] Image speedImg;
    [SerializeField] Image fastSpeedImg;
    [SerializeField] Image zoomInImg;
    [SerializeField] Image zoomOutImg;

    bool onAltScreen = false; // If a second screen is opened on the pause menu e.g. settings screen.
    [SerializeField] GameObject mainPauseCanvas;    // The main canvas of the pause screen
    [SerializeField] GameObject pauseCanvas;   // The initial screen of the pause screen
    [SerializeField] GameObject confirmCanvas;   // The confirmation screen for quitting the level
    [SerializeField] GameObject optionsCanvas;   // The options screen
    [SerializeField] GameObject pauseBG;   // BG for the pause screen
    [SerializeField] GameObject postProcess;   // Post-processing for the pause screen
    bool isTweenFinished = true;   // Checks if the pause tween is finished

    WaveManager waveManager;

    // Start is called before the first frame update
    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        pressToJoinTxt.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveCounter();

        pressToJoinTxt.enabled = (InputSystem.devices.Count > 3 && MultiplayerManager.playerCount < 2);
    }

    public void UpdateWaveCounter()
    {
        waveText.text = "Waves Remaining: " + waveManager.GetWavesRemaining().ToString();
    }

    public void UpdateCash(Dictionary<string, int> inventory)
    {
        cashText.text = inventory["Bits"].ToString();
        circuitText.text = inventory["Circuits"].ToString();
    }

    public void UpdateItemSlot(GameObject item)
    {
        Debug.Log("Depreceated function call");
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
                    Time.timeScale = currentTimeScale;
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

    // Speeds up the game
    public void FastForward()
    {
        if (!isPaused)
        {
            if (isFastForward)
            {
                currentTimeScale = 1f;
                speedImg.enabled = true;
                fastSpeedImg.enabled = false;
                Time.timeScale = currentTimeScale;
                isFastForward = !isFastForward;
            }
            else
            {
                currentTimeScale = 2.5f;
                speedImg.enabled = false;
                fastSpeedImg.enabled = true;
                Time.timeScale = currentTimeScale;
                isFastForward = !isFastForward;
            }
        }
    }

    // Changes icon in the zoom box
    public void Zoom(bool cur)
    {
        if (cur)
        {
            zoomInImg.enabled = true;
            zoomOutImg.enabled = false;
        }
        else
        {
            zoomInImg.enabled = false;
            zoomOutImg.enabled = true;
        }
    }

    // Opens the main pause menu
    // I.e. the one with a resume btn, main menu btn, etc.
    public void OpenMainPauseScreen()
    {
        confirmCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
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

    public void OpenOptionsScreen()
    {
        pauseCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
        onAltScreen = true;
    }
}
