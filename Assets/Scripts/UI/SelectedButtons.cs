using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectedButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShrinkButton();
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 0);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        ShrinkButton();
    }

    public void ShrinkButton()
    {
        transform.localScale = new Vector3(1f, 1f, 0);
    }
}
