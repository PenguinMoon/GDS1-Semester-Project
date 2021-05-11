using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchTurretBankSIngular : BaseListBank
{
    [SerializeField] public ListPositionCtrl listController;
    private int[] _contents = { 0 };
    //private string[] _contents = { "Turret1", "Turret2", "Turret3", "Turret4", "Turret5" };

    public override string GetListContent(int index)
    {
        return _contents[index].ToString();
    }

    public override int GetListLength()
    {
        return _contents.Length;
    }

    public Button GetListButton(int index)
    {
        return listController.listBoxes[index].GetComponent<Button>();
    }
}
