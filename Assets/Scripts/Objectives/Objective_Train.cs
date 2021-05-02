using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Train : MonoBehaviour
{
    Cinemachine.CinemachineDollyCart selfCart;

    [SerializeField] Cinemachine.CinemachineDollyCart linkCart;
    [SerializeField] Cinemachine.CinemachineDollyCart carriageCart;

    private void OnValidate()
    {
        selfCart = GetComponent<Cinemachine.CinemachineDollyCart>();
        selfCart.m_Position = 0;

        linkCart.m_Position = selfCart.m_Position - 7.5f;
        carriageCart.m_Position = selfCart.m_Position - 15f;

        linkCart.m_Speed = carriageCart.m_Speed = selfCart.m_Speed;
    }
}
