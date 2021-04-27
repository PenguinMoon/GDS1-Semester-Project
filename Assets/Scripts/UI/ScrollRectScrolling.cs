using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ScrollRectScrolling : MonoBehaviour
{
    [SerializeField] GameObject content;    // Container that includes ALL LvlBtns
    RectTransform contentRect;  // RectTransform of the content container
    [SerializeField] float offset;  // The amount of space inbetween each element required

    private void Awake()
    {
        contentRect = content.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Navigate(context);
    }

    private void Navigate(InputAction.CallbackContext context)
    {
        // Checks if a LvlBtn is selected before starting
        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name.Contains("LvlBtn"))
        {
            // On left input (i.e. x == -1), add a positive offset to the content's RectTransform
            if (context.ReadValue<Vector2>().x < 0)
            {
                contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x + offset, -100);
            }
            // On right input (i.e. x == 1), add a negative offset to the content's RectTransform
            else if (context.ReadValue<Vector2>().x > 0)
            {
                contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x - offset, -100);
            }
        }
    }
}
