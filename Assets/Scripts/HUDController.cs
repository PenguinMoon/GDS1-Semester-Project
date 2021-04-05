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
    [SerializeField] Slider staminaBar;
    [SerializeField] Gradient hpGradient;
    [SerializeField] Image hpImg;

    int maxWave = 10;
    List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    public bool isDraining, isRecovering;
    float sprintTime, maxSprintTime;

    // Start is called before the first frame update
    void Start()
    {
        foreach (EnemySpawner spawner in FindObjectsOfType<EnemySpawner>())
            enemySpawners.Add(spawner);
        isDraining = false;
        isRecovering = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveCounter();
        UpdateSprintBar();
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

    public void SetSprintDuration(float max)
    {
        sprintTime = max;
        maxSprintTime = max;
    }

    // Updates the sprint bar depending on its current settings
    void UpdateSprintBar()
    {
        if (isDraining)
        {
            isRecovering = false;
            sprintTime -= Time.deltaTime;
            float percent = sprintTime / maxSprintTime;
            
            staminaBar.value = Mathf.Lerp(0, 1, percent);
            if (sprintTime <= 0f)
            {
                sprintTime = 0f;
                Debug.Log("Sprint Bar Drained");
                isDraining = false;
            }    
        }
        else if (!isDraining && isRecovering)
        {
            sprintTime += Time.deltaTime;
            float percent = sprintTime / maxSprintTime;
            staminaBar.value = Mathf.Lerp(0, 1, percent);
            if (sprintTime >= maxSprintTime)
            {
                sprintTime = maxSprintTime;
                Debug.Log("Sprint Bar Filled");
                isRecovering = false;
            }
        }
    }
}
