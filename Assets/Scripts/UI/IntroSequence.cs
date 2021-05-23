using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] GameObject levelName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveLevelTxt());
    }

    IEnumerator MoveLevelTxt()
    {
        yield return new WaitForSeconds(2.0f);
        LeanTween.move(levelName.GetComponent<RectTransform>(), new Vector3(0, 0, 0), 1f).setEase(LeanTweenType.easeOutExpo);
        yield return new WaitForSeconds(8f);
        LeanTween.move(levelName.GetComponent<RectTransform>(), new Vector3(0, -Screen.height, 0), 1f).setEase(LeanTweenType.easeInOutExpo).setOnComplete(() =>
        {
            Destroy(this.gameObject);
        });

    }
}
