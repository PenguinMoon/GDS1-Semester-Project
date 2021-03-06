using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] GameObject levelName;
    PlayableDirector director;

    private void Awake()
    {
        director = GetComponentInChildren<PlayableDirector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveLevelTxt());
    }

    IEnumerator MoveLevelTxt()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        LeanTween.move(levelName.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 1f).setEase(LeanTweenType.easeOutExpo);
        yield return new WaitForSecondsRealtime(8f);
        LeanTween.move(levelName.GetComponent<RectTransform>(), new Vector3(0, -Screen.height, 0), .3f).setEase(LeanTweenType.easeInExpo).setOnComplete(() =>
        {
            Destroy(this.gameObject);
            FindObjectOfType<MultiplayerManager>().IntroSequenceFinished();
        });

    }

    public void SkipCutscene()
    {
        StopAllCoroutines();

        director.time = director.playableAsset.duration - 1f;   // Sets the timeline to be at the final second (which renables the HUD)
        director.Evaluate();
        
        LeanTween.move(levelName.GetComponent<RectTransform>(), new Vector3(0, -Screen.height, 0), .3f).setEase(LeanTweenType.easeInExpo).setOnComplete(() =>
        {
            Destroy(this.gameObject);
            FindObjectOfType<MultiplayerManager>().IntroSequenceFinished();
        });
    }

    public void OnSkip(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        SkipCutscene();
    }
}
