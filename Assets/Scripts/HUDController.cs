using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
    [SerializeField] TextMeshProUGUI scoreTxt;    // Timer text showing how long until next wave


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
}
