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
    public GameObject currBtn, nextBtn = null;
    [SerializeField] GameObject prevBtn = null;

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
        Debug.Log(nextBtn);
        if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name.Contains("LvlBtn"))
        {
            nextBtn = EventSystem.current.currentSelectedGameObject;
        }
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
                //contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x + offset, -100);
                
                LeanTween.move(contentRect, new Vector3(contentRect.anchoredPosition.x + offset, -100, 0), 0.4f).setOnComplete(FinishedTween).setEase(LeanTweenType.easeOutQuad);
                StartCoroutine(TweenBtn());

                //LeanTween.size(currBtn.GetComponent<RectTransform>(), new Vector2(280, 144), 0.5f);
                //LeanTween.size(nextBtn.GetComponent<RectTransform>(), new Vector2(350, 180), 0.5f);
            }
            // On right input (i.e. x == 1), add a negative offset to the content's RectTransform
            else if (context.ReadValue<Vector2>().x > 0)
            {
                //contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x - offset, -100);

                LeanTween.move(contentRect, new Vector3(contentRect.anchoredPosition.x - offset, -100, 0), 0.4f).setOnComplete(FinishedTween).setEase(LeanTweenType.easeOutQuad);
                StartCoroutine(TweenBtn());

                //LeanTween.size(currBtn.GetComponent<RectTransform>(), new Vector2(280, 144), 0.5f);
                //LeanTween.size(nextBtn.GetComponent<RectTransform>(), new Vector2(350, 180), 0.5f);
            }
        }
    }

    private void FinishedTween()
    {
        EventSystem.current.SetSelectedGameObject(nextBtn);
        //currBtn = nextBtn;
    }

    IEnumerator TweenBtn()
    {
        yield return new WaitForSeconds(0.0001f);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
