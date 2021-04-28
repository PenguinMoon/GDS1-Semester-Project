using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemListBox : ListBox
{
    public GameObject item;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI bitsPriceText;
    public TextMeshProUGUI circuitPriceText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UpdateDisplayContent()
    {
        // Update the content according to its contentID.
        itemNameText.text = item.GetComponent<Object>().objectName;
        bitsPriceText.text = item.GetComponent<Object>().bitsPrice.ToString();
        circuitPriceText.text = item.GetComponent<Object>().circuitsPrice.ToString();
    }
}
