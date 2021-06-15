using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintWordInfo 
{
    public string Word;
    public Vector2Int Position;
    public Vector2Int Direction;
    public int CurrentIndex;

    public HintWordInfo() {
        CurrentIndex = 0;
        Word = "";
        Position = new Vector2Int(-1, -1);
    }
    public HintWordInfo(string word, Vector2Int position, Vector2Int direction) {
        Word = word;
        Position = position;
        Direction = direction;
    }
    public Vector2Int GetNextCharPosition() {
        Debug.Log("Word: " + Word + " start pos: " + Position  + " direction: " + Direction + " index: " + CurrentIndex);
        Vector2Int res = Position + Direction * CurrentIndex;
        CurrentIndex++;
        return res;
    }
    public Vector2Int GetStartPosition() {
        return Position;
    }
    public Vector2Int GetEndPosition() {
        return Position + Direction * (Word.Length - 1);
    }
    public bool IsCompleted() {
        return CurrentIndex == Word.Length;
    }
    public override string ToString() {
        return $"Word: {Word} - Pos: {Position} - Direction: {Direction}";
    }
}
