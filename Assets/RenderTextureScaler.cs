using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureScaler : MonoBehaviour
{
    private Camera _attachedCamera;
    private RenderTexture _renderTexture;
    private float _screenRatio;
    private float _screenWidth;

    // Start is called before the first frame update
    void Start()
    {
        _attachedCamera = GetComponent<Camera>();
        _renderTexture = _attachedCamera.targetTexture;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (_screenRatio != (float)Screen.width / (float)Screen.height || _screenWidth != Screen.width)
    //        Scale();
    //}

    private void Scale()
    {
        _renderTexture.width = Screen.width;
        _renderTexture.height = Screen.height;
        _screenRatio = (float)Screen.width / (float)Screen.height;
        _screenWidth = Screen.width;
    }
}
