using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiSplitScreen : MonoBehaviour
{
    [SerializeField]
    private Transform[] _players;
    private DistancePair[] _playerDistances;
    private int _playerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform _player in _players)
        {
            if (_player != null) _playerCount++;
        }
        _playerDistances = new DistancePair[(_playerCount - 1) * (_playerCount - 2)];
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < _playerCount - 1; i++)
        {
            for(int j = i + 1; j < _playerCount; j++)
            {

            }
        }
    }
}

struct DistancePair
{
    int indexA;
    int indexB;

    float _distance;
}