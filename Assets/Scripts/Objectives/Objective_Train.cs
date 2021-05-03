using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Train : MonoBehaviour
{
    Cinemachine.CinemachineDollyCart selfCart;

    [SerializeField] Cinemachine.CinemachineDollyCart linkCart;
    [SerializeField] Cinemachine.CinemachineDollyCart carriageCart;

    [SerializeField] bool generateTrainTrack = false;
    [SerializeField] GameObject trainTrackObj;
    [SerializeField] List<GameObject> visualizedTrack = new List<GameObject>();
    Cinemachine.CinemachinePathBase track;

    private void OnValidate()
    {
        selfCart = GetComponent<Cinemachine.CinemachineDollyCart>();
        if (selfCart)
        {
            selfCart.m_Position = 0;

            linkCart.m_Position = selfCart.m_Position - 7.5f;
            carriageCart.m_Position = selfCart.m_Position - 15f;

            track = selfCart.m_Path;

            SetTrainSpeed(15);
        }

        if (generateTrainTrack)
            GenerateTrack();
    }

    public void SetTrainSpeed(float speed)
    {
        linkCart.m_Speed = carriageCart.m_Speed = selfCart.m_Speed = speed;
    }

    private void GenerateTrack()
    {
        foreach (GameObject g in visualizedTrack)
        {
            DestroyImmediate(g);
        }
        visualizedTrack.Clear();

        for (int i = 0; i < track.PathLength; i+=5)
        {
            GameObject obj = Instantiate(trainTrackObj, transform);
            Cinemachine.CinemachineDollyCart cart = obj.GetComponent<Cinemachine.CinemachineDollyCart>();
            cart.m_Path = track;
            cart.m_Position = i;
            visualizedTrack.Add(obj);
        }

        generateTrainTrack = false;
    }
}
