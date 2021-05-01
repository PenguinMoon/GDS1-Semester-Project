using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KioskObjective : MonoBehaviour
{
    [SerializeField] Text currencyAmountTxt;
    [SerializeField] int requiredBits = 0;
    [SerializeField] UnityEvent onObjectiveRepaired;

    int remainingBits = 0;

    private void Start()
    {
        remainingBits = requiredBits;

        //Assign amount to UI Element
        currencyAmountTxt.text = requiredBits.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered " + other);

        ReceiveBits(1);
    }

    private void ReceiveBits(int amount)
    {
        if (remainingBits > 0)
        {
            remainingBits -= amount;

            currencyAmountTxt.text = remainingBits.ToString();

            if (remainingBits <= 0)
                OnObjectiveFunded();
        }
    }

    private void OnObjectiveFunded()
    {
        onObjectiveRepaired.Invoke();
        Destroy(gameObject);
    }
}
