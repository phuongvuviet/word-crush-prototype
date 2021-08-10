using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCross;

[System.Serializable]
public class SessionDataModel 
{
    public List<string> SolveString;
    public List<char> WheelLetters;
    public BoardLayout Layout;
}

[System.Serializable]
public struct BoardLayoutRow
{
    public List<LetterModel> Letters;
}

[System.Serializable]
public struct BoardLayout
{
    public List<BoardLayoutRow> Layout;
}
