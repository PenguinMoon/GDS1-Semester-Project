using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndScreenMenu : MonoBehaviour
{
    [SerializeField] Button btn;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        btn.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        btn.OnSelect(null);
    }
}
