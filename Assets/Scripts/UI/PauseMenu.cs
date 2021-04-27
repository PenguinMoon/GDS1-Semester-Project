using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button btn;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
        btn.OnSelect(null);
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
