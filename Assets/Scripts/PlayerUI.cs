using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image staminaImg;
    [SerializeField] GameObject staminaGroup;
    public bool isDraining, isRecovering;
    float sprintTime, maxSprintTime;

    // Start is called before the first frame update
    void Start()
    {
        isDraining = false;
        isRecovering = false;
        staminaGroup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSprintBar();
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
            staminaGroup.SetActive(true);
            isRecovering = false;
            sprintTime -= Time.deltaTime;
            float percent = sprintTime / maxSprintTime;

            staminaImg.fillAmount = Mathf.Lerp(0, 1, percent);
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
            staminaImg.fillAmount = Mathf.Lerp(0, 1, percent);
            if (sprintTime >= maxSprintTime)
            {
                sprintTime = maxSprintTime;
                Debug.Log("Sprint Bar Filled");
                isRecovering = false;
                staminaGroup.SetActive(false);
            }
        }
    }
}
