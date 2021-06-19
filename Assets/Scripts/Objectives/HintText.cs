using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HintTextCollection", menuName = "Hint Text Collection", order = 52)]
public class HintText : ScriptableObject
{
    [TextArea(3, 5)]
    [SerializeField] string[] hints;

    public string[] Hints
    {
        get
        {
            return hints;
        }
    }
}
