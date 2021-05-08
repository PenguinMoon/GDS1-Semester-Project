using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Train : MonoBehaviour
{
    Cinemachine.CinemachineDollyCart selfCart;

    [SerializeField] Cinemachine.CinemachineDollyCart linkCart;
    [SerializeField] Cinemachine.CinemachineDollyCart carriageCart;


    [SerializeField] GameObject trainTrackObj;
    [SerializeField] Transform trackParent;
    [SerializeField] bool generateTrainTrack = false;
    List<GameObject> visualizedTrack = new List<GameObject>();
    Cinemachine.CinemachinePathBase track;

    [SerializeField] ParticleSystem smokeParticle;
    [SerializeField] SmartTurret trainTurret;
    [SerializeField] List<GameObject> trainTurretPlatforms;

    private void Awake()
    {
        GenerateTrack();
        trainTurret.repaired = false;

        foreach (GameObject g in trainTurretPlatforms)
            g.SetActive(false);
    }

    private void OnValidate()
    {
        selfCart = GetComponent<Cinemachine.CinemachineDollyCart>();
        if (selfCart)
        {
            selfCart.m_Position = 0;

            linkCart.m_Position = selfCart.m_Position - 7.5f;
            carriageCart.m_Position = selfCart.m_Position - 15f;

            track = selfCart.m_Path;

            SetTrainSpeed(0);
        }

        if (generateTrainTrack)
            GenerateTrack();
    }

    public void SetTrainSpeed(float speed)
    {
        linkCart.m_Speed = carriageCart.m_Speed = selfCart.m_Speed = speed;

        if (speed > 0)
        {
            smokeParticle.Play();
            trainTurret.repaired = true;

            foreach (GameObject g in trainTurretPlatforms)
                g.SetActive(true);
        }

    }

    private void GenerateTrack()
    {
        foreach (GameObject g in visualizedTrack)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (g != null)
                    DestroyImmediate(g);
            };
        }
        visualizedTrack.Clear();

        for (int i = 0; i < track.PathLength; i += 5)
        {
            GameObject obj = Instantiate(trainTrackObj, trackParent);
            Cinemachine.CinemachineDollyCart cart = obj.GetComponent<Cinemachine.CinemachineDollyCart>();
            cart.m_Path = track;
            cart.m_Position = i;
            visualizedTrack.Add(obj);
        }

        generateTrainTrack = false;
    }
}
