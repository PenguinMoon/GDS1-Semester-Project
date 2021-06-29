using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class HUDController : MonoBehaviour
{
    // ==== HUD Elements ====
    [Header("HUD Elements")]
    [SerializeField] TextMeshProUGUI cashText;
    [SerializeField] TextMeshProUGUI circuitText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] Slider hpBar;
    [SerializeField] Gradient hpGradient;
    [SerializeField] Image hpImg;
    [SerializeField] Image speedImg;
    [SerializeField] Image fastSpeedImg;
    [SerializeField] Image zoomInImg;
    [SerializeField] Image zoomOutImg;
    bool isAlertActive; // Checks if the alert is currently active
    float alertTimer = 15f; // Cooldown for showing the alert
    [SerializeField] GameObject damageAlert;    // Image + text of the workshop damage alert
    [SerializeField] TextMeshProUGUI waveTimerTxt;    // Timer text showing how long until next wave
    float waveDelayTime;
    [SerializeField] TextMeshProUGUI scoreTxt;    // Text showing score


    // ==== Multiplayer Related Elements ====
    [Header("Multiplayer Related Elements")]
    [SerializeField] TextMeshProUGUI pressToJoinTxt;
    [SerializeField] GameObject p2Icon;

    // ==== Pause Screen Elements ====
    bool isPaused = false;
    bool isFastForward = false;
    float currentTimeScale = 1.0f;
    bool onAltScreen = false; // If a second screen is opened on the pause menu e.g. settings screen.
    [Header("Pause Screen Elements")]
    [SerializeField] GameObject mainPauseCanvas;    // The main canvas of the pause screen
    [SerializeField] GameObject pauseCanvas;   // The initial screen of the pause screen
    [SerializeField] GameObject confirmCanvas;   // The confirmation screen for quitting the level
    [SerializeField] GameObject optionsCanvas;   // The options screen
    [SerializeField] GameObject pauseBG;   // BG for the pause screen
    [SerializeField] GameObject postProcess;   // Post-processing for the pause screen
    bool isTweenFinished = true;   // Checks if the pause tween is finished

    // ==== End Screen Elements ====
    [Header("End Screen Elements")]
    [SerializeField] GameObject endCanvas;   // The canvas after the level is over
    [SerializeField] TextMeshProUGUI endTitleTxt;    // Title text on end screen
    [SerializeField] TextMeshProUGUI endScoreTxt;    // Text showing score on end screen
    [SerializeField] GameObject highScoreTxt;    // Text showing score on end screen
    [SerializeField] GameObject scoreboard;   // The scoreboard after the level is over
    [SerializeField] Image starImg;   // The scoreboard after the level is over
    [SerializeField] GameObject retryBtn;

    WaveManager waveManager;
    WaveManagerV2 waveManagerV2;

    // Start is called before the first frame update
    void Start()
    {
        //waveManager = FindObjectOfType<WaveManager>();
        waveManagerV2 = FindObjectOfType<WaveManagerV2>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveCounter();
        UpdateWaveTimer();

        //pressToJoinTxt.enabled = (InputSystem.devices.Count > 3 && MultiplayerManager.playerCount < 2);
       
        // Disables press to join txt + enables P2 Icon
        // TODO: Find a more efficient way to do this.
        if(InputSystem.devices.Count > 3 && MultiplayerManager.playerCount == 2)
        {
            pressToJoinTxt.enabled = false;
            p2Icon.SetActive(true);
        }

        alertTimer -= Time.deltaTime;
        if (alertTimer < 0f)
            alertTimer = 0f;
    }

    public void UpdateWaveCounter()
    {
        //waveText.text = "Waves Remaining: " + waveManager.GetWavesRemaining().ToString();
        //waveText.text = "Waves Remaining: " + waveManagerV2.GetWavesRemaining().ToString();
        waveText.text = "Wave: " + waveManagerV2.GetCurrentWave();
    }

    void UpdateWaveTimer()
    {
        if (!waveManagerV2.IsWaveInProgress() && waveManagerV2.hasLevelStarted)
        {
            waveTimerTxt.enabled = true;
            // Counts down the wave timer
            waveDelayTime -= Time.deltaTime;
            waveTimerTxt.text = "Next wave in: " + waveDelayTime.ToString("F0");
        }
        else
        {
            waveDelayTime = waveManagerV2.GetWaveDelay();   // Resets wave delay time back
            waveTimerTxt.enabled = false;
        }
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
        ActivateDamageAlert();
    }

    public void UpdateScoreTxt(float score)
    {
        scoreTxt.text = "Score: " + score.ToString("F0");
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

    // Displays a damage alert when the workshop takes damage
    void ActivateDamageAlert()
    {
        // Only activates if the alert is not already active + cooldown is off
        if(!isAlertActive && alertTimer <= 0)
        {
            isAlertActive = true;
            LeanTween.move(damageAlert.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 1f).setEase(LeanTweenType.easeOutExpo);
            // Moves the alert off screen and resets the alert's cooldown + state
            LeanTween.move(damageAlert.GetComponent<RectTransform>(), new Vector3(0, -Screen.height, 0), .5f).setEase(LeanTweenType.easeInExpo).setDelay(4f).setOnComplete(() =>
            {
                isAlertActive = false;
                alertTimer = 15f;
            });
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

    // Display the game over screen + scoreboard when the level is over
    public void EndGame(bool isLevelWon)
    {
        LeanTween.value(currentTimeScale, 0f, 1f).setIgnoreTimeScale(true).setOnUpdate((float value) =>
        {
            Time.timeScale = value;
        });

        mainPauseCanvas.SetActive(true);
        postProcess.SetActive(true);
        pauseBG.SetActive(true);
        endCanvas.SetActive(true);
        waveManagerV2.hasLevelStarted = false;

        // Change title text depending on if the level was won or lost
        if (isLevelWon)
            endTitleTxt.text = "Level Complete!";
        else
            endTitleTxt.text = "Game Over!";

        endScoreTxt.text = MultiplayerManager.Score.ToString("F0");

        // Shows the star if the level has been cleared with no damage taken
        // Else, have the colour of the star be in black
        if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "Star", 0) == 1)
            starImg.color = new Color(1f, 1f, 1f);
        else
            starImg.color = new Color(0f, 0f, 0f);

        Debug.LogWarning("Final score saved");

        if (PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name, 0) < MultiplayerManager.Score)
        {
            PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, MultiplayerManager.Score);
            highScoreTxt.SetActive(true);
        }

        LeanTween.scale(endTitleTxt.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutElastic).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            LeanTween.scale(scoreboard.GetComponent<RectTransform>(), new Vector3(1, 1, 1), .5f).setEase(LeanTweenType.easeOutQuad).setIgnoreTimeScale(true).setDelay(0.5f).setOnComplete(() =>
            {
                EventSystem.current.SetSelectedGameObject(retryBtn);
            });
        });
    }
}
