using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquareLayout
{
    // Has to be size = 9
    public List<string> layout;
}

[CreateAssetMenu(fileName = "Data", menuName = "KeybaordLayout", order = 1)]
public class KeyboardLayout : ScriptableObject
{
    // Has to be size 9
    public List<SquareLayout> squares;

    public string GetInput(int primary, int secondary)
    {
        return squares[primary].layout[secondary];
    }
}
