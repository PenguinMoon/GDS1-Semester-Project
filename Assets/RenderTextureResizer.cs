using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureResizer : MonoBehaviour
{
    private Resolution _screenResolution;
    private RenderTexture _renderTexture;

    // Start is called before the first frame update
    void Start()
    {
        _screenResolution = Screen.currentResolution;
        _renderTexture = GetComponent<Camera>().targetTexture;
        _renderTexture.width = _screenResolution.width;
        _renderTexture.height = _screenResolution.height;
    }

    // Update is called once per frame
    void Update()
    {
        if(_screenResolution.width != Screen.currentResolution.width ||
            _screenResolution.height != Screen.currentResolution.height)
        {
            _screenResolution = Screen.currentResolution;
            _renderTexture.width = _screenResolution.width;
            _renderTexture.height = _screenResolution.height;
        }
    }

    
}
