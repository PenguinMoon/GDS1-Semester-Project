using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchItemBank : BaseListBank
{

    [SerializeField] public ListPositionCtrl listController;
    private int[] _contents = { 0, 1 };

    public override string GetListContent(int index)
    {
        return _contents[index].ToString();
    }

    public override int GetListLength()
    {
        return _contents.Length;
    }
}
