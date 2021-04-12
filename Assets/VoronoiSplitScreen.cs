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
        int index = 0;
        for(int i = 0; i < _playerCount - 1; i++)
        {
            for(int j = i + 1; j < _playerCount; j++)
            {
                _playerDistances[index] = new DistancePair();
                _playerDistances[index]._indexA = i;
                _playerDistances[index]._indexB = j;
                _playerDistances[index]._distance = Vector3.Distance(_players[i].position, _players[j].position);
                _playerDistances[index]._midPoint = Vector3.Lerp(_players[i].position, _players[j].position, 0.5f);
                _playerDistances[index]._normal = (_players[j].position - _players[i].position).normalized;
            }
        }
    }

    private int TriangleNumberSequence(int n)
    {
        return n * (n + 1) / 2;
    }
}

struct DistancePair
{
    public int _indexA;
    public int _indexB;

    public float _distance;
    public Vector3 _midPoint;
    public Vector3 _normal;
}