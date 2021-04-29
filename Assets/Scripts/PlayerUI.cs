using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image staminaImg;
    [SerializeField] GameObject staminaGroup;
    public bool isDraining, isRecovering;
    float sprintTime, maxSprintTime;

    [SerializeField] Canvas HintTextUI;
    TextMeshProUGUI HintText;
    public bool HintEnabled;


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

    public void UpdateHintText(string hint)
    {
        HintTextUI.GetComponentInChildren<TextMeshProUGUI>().text = hint;
    }

    public void ShowHintUIText()
    {
        HintTextUI.enabled = true;
        // Debug.Log("SHOWING UI");
/*        if (!HintEnabled)
        {
            HintTextUI.transform.LeanScale(new Vector3(1, 1, 1), 0.2f).setEaseInOutExpo();
            HintEnabled = true;
        }*/

    }

    public void HideHintUIText()
    {
        HintTextUI.enabled = false;
        //Debug.Log("HIDING UI");
        /*if (HintEnabled)
        {
            HintTextUI.transform.LeanScale(new Vector3(0, 0, 1), 0.2f).setEaseInOutExpo();
            HintEnabled = false;
        }*/
    }
}
