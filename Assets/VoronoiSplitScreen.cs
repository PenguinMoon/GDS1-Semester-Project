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
                Vector2 _pointA = new Vector2(_players[i].position.x, _players[i].position.z);
                Vector2 _pointB = new Vector2(_players[j].position.x, _players[j].position.z);

                _playerDistances[index]._distance = Vector2.Distance(_pointA, _pointB);
                _playerDistances[index]._midPoint = Vector2.Lerp(_pointA, _pointB, 0.5f);
                _playerDistances[index]._normal = (_pointB - _pointA).normalized;
                
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
    public Vector2 _midPoint;
    public Vector2 _normal;
}