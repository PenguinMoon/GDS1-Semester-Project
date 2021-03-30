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

    int maxWave = 10;
    List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (EnemySpawner spawner in FindObjectsOfType<EnemySpawner>())
            enemySpawners.Add(spawner);
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
}
