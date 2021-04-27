using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{

    public Material material;
    public RenderTexture RenderTexture;

    public Transform _player1;
    public Transform _player2;

    private float _playerDistance;
    private Vector2 _playerDirection;
    private Vector2 _playerPerpendicular;
    private float _riseOverRun;

    // Start is called before the first frame update
    void Start()
    {
        CalculatePerpendicular();
        _riseOverRun = _playerPerpendicular.y / _playerPerpendicular.x;
        material.SetTexture("_SecondScreenTex", RenderTexture);
        material.SetFloat("_LineAngle", _riseOverRun);
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePerpendicular();
        _riseOverRun = _playerPerpendicular.y / _playerPerpendicular.x;
        material.SetFloat("_LineAngle", _riseOverRun);
    }

    private void CalculatePerpendicular()
    {
        Vector2 _player1Pos = new Vector2(_player1.position.x, _player1.position.z),
            _player2Pos = new Vector2(_player2.position.x, _player2.position.z);
        _playerDistance = Vector2.Distance(_player1Pos, _player2Pos);
        _playerDirection = _player2Pos - _player1Pos;
        _playerPerpendicular = Vector2.Perpendicular(_playerDirection);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_playerDistance > 5)
        {
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
